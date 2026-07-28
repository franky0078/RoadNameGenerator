using Baubeschreibung.Klassen;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Baubeschreibung
{
    public partial class Eingabe : Page
    {
        private readonly Datenquelle _quelle;
        // Die RichTextBox, in der gerade geschrieben wird (null = keine aktiv).
        private RichTextBox _aktiveBox;

        // Merkt sich pro Zeile die zugehörige RichTextBox. Wird gebraucht, damit
        // "Text aus Projekt übernehmen" weiß, wohin es schreiben soll.
        private readonly Dictionary<ZeileVM, RichTextBox> _boxen = new();

        // Unterdrückt das Zurückschreiben, während wir selbst das Dokument befüllen.
        private bool _befuellt;

        // Vorgemerkte Formatierung für den Fall "Dokument noch leer".
        // WPF verwirft seine eigene Vormerkung, sobald beim ersten Tastendruck
        // die Textstruktur aufgebaut wird. Deshalb merken wir sie uns selbst
        // und wenden sie in Rtb_TextChanged auf den frisch getippten Text an.
        private bool? _vorFett, _vorKursiv, _vorUnterstrichen;

        // Die einfache TextBox (Anzeige-Modus), in der gerade der Cursor steht.
        private TextBox _aktiveTextBox;

        // Zuletzt benutztes Textfeld – bewusst als Control, damit BEIDE Feldarten
        // hineinpassen. Ein getrenntes _letzteBox/_letzteTextBox wäre eine Falle:
        // beide könnten gleichzeitig gefüllt sein und man wüsste nicht, welches
        // das jüngere ist -> das Zeichen landet im falschen Feld.
        private Control _letztesFeld;
        private enum Auszeichnung { Fett, Kursiv, Unterstrichen }

        public ObservableCollection<TitelVM> Titel { get; } = new();

        public Eingabe()
        {
            InitializeComponent();

            _quelle = new Datenquelle(AktiverBereich.Nummer);

            AktiverBereich.Geaendert -= BereichGewechselt;
            AktiverBereich.Geaendert += BereichGewechselt;

            TitelAusDatenbankLaden();
            TitelListe.ItemsSource = Titel;
            Loaded += Seite_Geladen;
        }

        /// <summary>
        /// Stellt die Seite auf ein anderes Leistungsverzeichnis um.
        /// neuAufbauen = true  -> der Benutzer hat gewechselt: der Titelbaum wird
        ///                        neu aufgebaut, weil die bisherigen Positionen
        ///                        zum alten LV gehören.
        /// neuAufbauen = false -> das Programm hat den Bereich beim Laden eines
        ///                        Projekts gesetzt: die geladenen Positionen
        ///                        MÜSSEN stehenbleiben, nur die Namenslisten
        ///                        und aufgelösten Texte werden nachgezogen.
        /// </summary>
        private void BereichGewechselt(bool neuAufbauen)
        {
            // NeuLaden statt neues Objekt: alle View-Models halten eine
            // Referenz auf genau diese Datenquelle.
            _quelle.NeuLaden(AktiverBereich.Nummer);

            if (neuAufbauen)
            {
                TitelAusDatenbankLaden();
                return;
            }

            foreach (var t in Titel)
                foreach (var u in t.Untertitel)
                    foreach (var z in u.Zeilen)
                        z.NamensListeAktualisieren();
        }

        /// <summary>
        /// Enthält die Seite überhaupt Positionen? Wird vom MainWindow
        /// gebraucht, um vor einem Bereichswechsel nur dann nachzufragen,
        /// wenn tatsächlich etwas verloren gehen kann.
        /// </summary>
        public bool HatInhalt() => Titel.Any(t => t.Untertitel.Count > 0);

        // Lädt die Titel 2 bis 8 aus den in der Optionen-Seite gespeicherten Feldern.
        // Titel 1 ("Allgemein") gehört auf die Einleitung-Seite und wird hier ausgelassen.
        // Standard-Titel als Rückfallebene, falls in den Optionen noch nichts gespeichert wurde.
        // Index passend zu den Optionen-Schlüsseln: [2] = "Rohbau", [3] = "Innenausbau" usw.
        private static readonly Dictionary<int, (string nr, string name)> StandardTitel = new()
        {
            { 2, ("2", "Rohbau") },
            { 3, ("3", "Innenausbau") },
            { 4, ("4", "Aussenanlagen") },
            { 5, ("5", "Sonstiges") },
            { 6, ("6", "Bauherrenaufgaben") },
            { 7, ("7", "Sonstiges") },
            { 8, ("8", "Bauherrenaufgaben") },
        };

        // Koppelt die Höhe des RootGrid an die sichtbare Höhe des äußeren
        // ScrollViewers im MainWindow. Dadurch füllt die Seite exakt den
        // Bildschirm, die Kopfzeile bleibt stehen, und nur der innere
        // ScrollViewer um die Titelliste scrollt. (Gleiches Muster wie auf
        // der Datenbank-Seite.)
        private void Seite_Geladen(object sender, RoutedEventArgs e)
        {
            var aussen = ElternSuchen<ScrollViewer>(this);
            if (aussen == null) return;   // kein äußerer ScrollViewer -> nichts zu tun

            var bindung = new System.Windows.Data.Binding(nameof(ScrollViewer.ViewportHeight))
            {
                Source = aussen
            };
            RootGrid.SetBinding(HeightProperty, bindung);
        }

        // Läuft den visuellen Baum nach oben, bis ein Element vom gesuchten Typ kommt.
        private static T? ElternSuchen<T>(DependencyObject von) where T : DependencyObject
        {
            var aktuell = VisualTreeHelper.GetParent(von);
            while (aktuell != null && aktuell is not T)
                aktuell = VisualTreeHelper.GetParent(aktuell);
            return aktuell as T;
        }

        private void TitelAusDatenbankLaden()
        {
            Titel.Clear();

            using var db = new AppDbContext();
            var werte = db.Einstellungen.ToDictionary(t => t.Schluessel, t => t.Wert);

            for (int i = 2; i <= 8; i++)
            {
                werte.TryGetValue($"Optionen_Titel{i}_Nr", out var nr);
                werte.TryGetValue($"Optionen_Titel{i}_Name", out var name);

                nr = (nr ?? "").Trim();
                name = (name ?? "").Trim();

                // Fallback: fehlt der DB-Wert, nimm den Standard-Titel für diesen Index
                if (nr.Length == 0 && name.Length == 0)
                {
                    if (StandardTitel.TryGetValue(i, out var standard))
                    {
                        nr = standard.nr;
                        name = standard.name;
                    }
                    else
                    {
                        continue;   // für diesen Index gibt es keinen Standard -> überspringen
                    }
                }

                string titelText = nr.Length > 0 ? $"{nr}. {name}" : name;

                Titel.Add(new TitelVM(_quelle, titelText));
            }
        }

        private void OptionenUebernehmen_Click(object sender, RoutedEventArgs e)
        {
            var antwort = MessageBox.Show(
                "Möchtest du die Titel aus den Optionen neu übernehmen?\n\n" +
                "Achtung: Bereits hinzugefügte Untertitel und Zeilen auf dieser Seite " +
                "gehen dabei verloren.",
                "Einstellungen übernehmen",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (antwort == MessageBoxResult.Yes)
                TitelAusDatenbankLaden();
        }

        private void SeiteLeeren_Click(object sender, RoutedEventArgs e)
        {
            var antwort = MessageBox.Show(
                "Möchtest du diese Seite komplett leeren?\n\n" +
                "Alle hinzugefügten Untertitel und Zeilen werden entfernt. " +
                "Die Titel-Abschnitte bleiben erhalten.\n\n" +
                "Achtung: Nicht gespeicherte Eingaben auf dieser Seite gehen verloren.",
                "Seite leeren",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (antwort == MessageBoxResult.Yes)
                InhaltLeeren();
        }

        public void InhaltLeeren()
        {
            foreach (var t in Titel)
            {
                t.Untertitel.Clear();   // löscht alle Untertitel + deren Zeilen
                t.IstAktiv = true;      // Abschnitte wieder auf "JA" (aufgeklappt)
            }
        }

        private void UntertitelHinzufuegen_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is TitelVM t)
                t.UntertitelHinzufuegen();
        }

        private void UntertitelEntfernen_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is UntertitelVM u)
                u.Entferne();
        }

        private void ZeileHinzufuegen_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is UntertitelVM u)
                u.ZeileHinzufuegen();
        }

        private void ZeileEntfernen_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is ZeileVM z)
                z.Entferne();
        }


        // ═══════════════════════════════════════════════════════════
        // Texteingabe: RichTextBox <-> ViewModel
        // ═══════════════════════════════════════════════════════════

        // Zwingt ein Dokument auf die Schrift aus den Page-Ressourcen.
        // Nötig, weil geladene XamlPackages ihre eigenen Schriftangaben als
        // LOKALE Werte auf den Runs mitbringen – die schlagen jede Vererbung
        // vom Steuerelement.
        // Rückgabe: true = es wurde tatsächlich etwas angeglichen.
        private bool SchriftVereinheitlichen(FlowDocument doc)
        {
            // Der Standard-Innenabstand des FlowDocuments erzeugt sonst einen
            // zusätzlichen Einzug, den die einfache TextBox nicht hat.
            doc.PagePadding = new Thickness(0);

            doc.FontFamily = (FontFamily)FindResource("FeldSchrift");
            doc.FontSize = (double)FindResource("FeldSchriftGroesse");

            var alles = new TextRange(doc.ContentStart, doc.ContentEnd);
            if (alles.Text.Trim('\r', '\n').Length == 0)
                return false;   // leeres Dokument -> nichts anzugleichen

            // Stimmt schon alles? GetPropertyValue liefert UnsetValue, wenn der
            // Bereich uneinheitlich ist – dann greift die Prüfung ebenfalls nicht
            // und wir gleichen an. Genau das gewünschte Verhalten.
            bool passt =
                alles.GetPropertyValue(TextElement.FontFamilyProperty) is FontFamily f
                && f.Source == doc.FontFamily.Source
                && alles.GetPropertyValue(TextElement.FontSizeProperty) is double g
                && Math.Abs(g - doc.FontSize) < 0.01;

            if (passt) return false;

            // Setzt Schriftart und -größe auf ALLE Textteile. Fett, Kursiv,
            // Unterstrichen und Aufzählungen bleiben unberührt – das sind
            // andere Eigenschaften.
            alles.ApplyPropertyValue(TextElement.FontFamilyProperty, doc.FontFamily);
            alles.ApplyPropertyValue(TextElement.FontSizeProperty, doc.FontSize);
            return true;
        }

        // Speichert den Boxinhalt ins ViewModel (formatiert + Klartext).
        // Ausgelagert, weil jetzt zwei Stellen das brauchen.
        private static void ZurueckSchreiben(RichTextBox box, ZeileVM z)
        {
            var bereich = new TextRange(box.Document.ContentStart, box.Document.ContentEnd);

            using var ms = new MemoryStream();
            bereich.Save(ms, DataFormats.XamlPackage);
            z.EigenerTextFormatiert = Convert.ToBase64String(ms.ToArray());
            z.EigenerText = bereich.Text.TrimEnd('\r', '\n');
        }

        private void Rtb_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is not RichTextBox box || box.DataContext is not ZeileVM z) return;

            _boxen[z] = box;

            bool angeglichen = false;

            _befuellt = true;
            try
            {
                var doc = new FlowDocument();

                if (!string.IsNullOrWhiteSpace(z.EigenerTextFormatiert))
                {
                    try
                    {
                        var bytes = Convert.FromBase64String(z.EigenerTextFormatiert);
                        using var ms = new MemoryStream(bytes);
                        new TextRange(doc.ContentStart, doc.ContentEnd)
                            .Load(ms, DataFormats.XamlPackage);
                    }
                    catch { doc = new FlowDocument(); }
                }
                else if (!string.IsNullOrEmpty(z.EigenerText))
                {
                    doc = new FlowDocument(new Paragraph(new Run(z.EigenerText)));
                }

                box.Document = doc;
                angeglichen = SchriftVereinheitlichen(doc);
            }
            finally { _befuellt = false; }

            // Wurde die Schrift korrigiert, den gespeicherten Stand nachziehen –
            // sonst zeigen Druckausgabe und Savegame weiter die alte Schrift,
            // obwohl der Editor schon richtig aussieht.
            if (angeglichen)
                ZurueckSchreiben(box, z);
        }

        private void Rtb_Unloaded(object sender, RoutedEventArgs e)
        {
            if (sender is RichTextBox box && box.DataContext is ZeileVM z)
                _boxen.Remove(z);

            if (ReferenceEquals(_letztesFeld, sender))
                _letztesFeld = null;

            if (ReferenceEquals(_aktiveBox, sender))
            {
                _aktiveBox = null;
                WerkzeugeAktualisieren();
            }
        }

        private void Rtb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_befuellt) return;
            if (sender is not RichTextBox box || box.DataContext is not ZeileVM z) return;

            // Vorgemerkte Formatierung auf den gerade entstandenen Text anwenden.
            if (_vorFett.HasValue || _vorKursiv.HasValue || _vorUnterstrichen.HasValue)
            {
                _befuellt = true;   // verhindert, dass das eigene Ändern hier erneut hereinläuft
                try
                {
                    var alles = new TextRange(box.Document.ContentStart, box.Document.ContentEnd);

                    if (_vorFett.HasValue)
                        alles.ApplyPropertyValue(TextElement.FontWeightProperty,
                            _vorFett.Value ? FontWeights.Bold : FontWeights.Normal);

                    if (_vorKursiv.HasValue)
                        alles.ApplyPropertyValue(TextElement.FontStyleProperty,
                            _vorKursiv.Value ? FontStyles.Italic : FontStyles.Normal);

                    if (_vorUnterstrichen.HasValue)
                        alles.ApplyPropertyValue(Inline.TextDecorationsProperty,
                            _vorUnterstrichen.Value ? TextDecorations.Underline : null);
                }
                finally { _befuellt = false; }

                VormerkungLeeren();
                box.CaretPosition = box.Document.ContentEnd;   // Cursor hinter den Text
                ZurueckSchreiben(box, z);
            }

            var bereich = new TextRange(box.Document.ContentStart, box.Document.ContentEnd);

            using var ms = new MemoryStream();
            bereich.Save(ms, DataFormats.XamlPackage);
            z.EigenerTextFormatiert = Convert.ToBase64String(ms.ToArray());
            z.EigenerText = bereich.Text.TrimEnd('\r', '\n');
        }

        // ── Einfache TextBox (Anzeige-Modus): Fokus verfolgen ──
        private void Txt_GotFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            _aktiveTextBox = sender as TextBox;
            WerkzeugeAktualisieren();
        }

        private void Txt_LostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (ReferenceEquals(_aktiveTextBox, sender))
            {
                _letztesFeld = _aktiveTextBox;
                _aktiveTextBox = null;
                WerkzeugeAktualisieren();
            }
        }

        // Verhindert, dass wir auf ein Feld zeigen, das gar nicht mehr in der
        // Oberfläche hängt (Zeile entfernt, Seite gewechselt).
        private void Txt_Unloaded(object sender, RoutedEventArgs e)
        {
            if (ReferenceEquals(_letztesFeld, sender))
                _letztesFeld = null;

            if (ReferenceEquals(_aktiveTextBox, sender))
            {
                _aktiveTextBox = null;
                WerkzeugeAktualisieren();
            }
        }

        // ═══════════════════════════════════════════════════════════
        // Werkzeugleiste: Zustand und Formatierung
        // ═══════════════════════════════════════════════════════════

        private void Rtb_GotFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            _aktiveBox = sender as RichTextBox;
            VormerkungLeeren();
            WerkzeugeAktualisieren();
        }

        private void Rtb_LostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // Greift nur, wenn der Fokus wirklich weg ist. Die Werkzeug-Schaltflächen
            // stehen auf Focusable="False" und lösen das hier deshalb nicht aus.
            if (ReferenceEquals(_aktiveBox, sender))
            {
                _letztesFeld = _aktiveBox;
                _aktiveBox = null;
                VormerkungLeeren();
                WerkzeugeAktualisieren();
            }
        }

        private void Rtb_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (ReferenceEquals(_aktiveBox, sender))
                WerkzeugeAktualisieren();
        }

        // Schaltflächen ein-/ausschalten und ihren Gedrückt-Zustand an die
        // aktuelle Cursorposition bzw. Markierung angleichen.
        // Schaltflächen ein-/ausschalten und ihren Gedrückt-Zustand an die
        // aktuelle Cursorposition bzw. Markierung angleichen.
        private void WerkzeugeAktualisieren()
        {
            bool reich = _aktiveBox != null;        // RichTextBox: kann Formatierung
            bool einfach = _aktiveTextBox != null;    // TextBox: nur Klartext

            // Formatierung gibt es nur in der RichTextBox – die einfache TextBox
            // speichert einen reinen string, dort wäre Fett schlicht nicht ablegbar.
            BtnFett.IsEnabled = reich;
            BtnKursiv.IsEnabled = reich;
            BtnUnterstrichen.IsEnabled = reich;
            BtnListe.IsEnabled = reich;

            // Pfeile sind normale Zeichen -> funktionieren in BEIDEN Feldarten.
            BtnPfeilLinks.IsEnabled = reich || einfach;
            BtnPfeilRechts.IsEnabled = reich || einfach;

            if (!reich)
            {
                BtnFett.IsChecked = false;
                BtnKursiv.IsChecked = false;
                BtnUnterstrichen.IsChecked = false;
                BtnListe.IsChecked = false;
                return;
            }

            var aus = _aktiveBox.Selection;

            BtnFett.IsChecked = _vorFett ??
                (aus.GetPropertyValue(TextElement.FontWeightProperty) is FontWeight fw
                 && fw == FontWeights.Bold);

            BtnKursiv.IsChecked = _vorKursiv ??
                (aus.GetPropertyValue(TextElement.FontStyleProperty) is FontStyle fs
                 && fs == FontStyles.Italic);

            BtnUnterstrichen.IsChecked = _vorUnterstrichen ??
                (aus.GetPropertyValue(Inline.TextDecorationsProperty) is TextDecorationCollection td
                 && td.Count > 0);

            // Liste: Absatz am Auswahlanfang ansehen. Steckt er in einem ListItem
            // einer List mit Punkt-Markern, ist die Aufzählung aktiv. Keine
            // Vormerkung nötig (siehe Liste_Click) -> direkt aus dem Dokument lesen.
            Paragraph absatz = aus.Start.Paragraph;
            BtnListe.IsChecked =
                absatz?.Parent is ListItem eintrag &&
                eintrag.Parent is List liste &&
                liste.MarkerStyle == TextMarkerStyle.Disc;
        }

        private void Fett_Click(object sender, RoutedEventArgs e)
            => FormatUmschalten(Auszeichnung.Fett);

        private void Kursiv_Click(object sender, RoutedEventArgs e)
            => FormatUmschalten(Auszeichnung.Kursiv);

        private void Unterstrichen_Click(object sender, RoutedEventArgs e)
            => FormatUmschalten(Auszeichnung.Unterstrichen);

        private void Liste_Click(object sender, RoutedEventArgs e)
        {
            var box = _aktiveBox;
            if (box == null) return;

            // Anders als F/K/U braucht die Liste KEINE Vormerkung: ToggleBullets
            // arbeitet strukturell auf Absatzebene und baut das List/ListItem-
            // Gerüst sofort um den (auch leeren) Absatz. Es gibt nichts, was
            // der Editor beim ersten Tippen verwerfen könnte.
            if (EditingCommands.ToggleBullets.CanExecute(null, box))
                EditingCommands.ToggleBullets.Execute(null, box);

            WerkzeugeAktualisieren();
        }

        private void PfeilLinks_Click(object sender, RoutedEventArgs e)
            => ZeichenEinfuegen("←");

        private void PfeilRechts_Click(object sender, RoutedEventArgs e)
            => ZeichenEinfuegen("→");

        // ═══════════════════════════════════════════════════════════
        // Sonderzeichen (Pfeile) an der Cursorposition einfügen
        // ═══════════════════════════════════════════════════════════

        /// Schreibt ein Zeichen dorthin, wo der Cursor gerade steht – egal ob in
        // einer RichTextBox (Texteingabe) oder in der einfachen TextBox (Anzeige).
        private void ZeichenEinfuegen(string zeichen)
        {
            if (string.IsNullOrEmpty(zeichen)) return;

            // Reihenfolge ist entscheidend: erst die TATSÄCHLICH fokussierten Felder,
            // erst danach das gemerkte Rückfall-Feld. Sonst landet das Zeichen in der
            // zuletzt benutzten Box, obwohl der Cursor längst woanders steht.
            if (_aktiveBox != null) { InRichTextEinfuegen(_aktiveBox, zeichen); return; }
            if (_aktiveTextBox != null) { InTextBoxEinfuegen(_aktiveTextBox, zeichen); return; }

            if (_letztesFeld is RichTextBox r) InRichTextEinfuegen(r, zeichen);
            else if (_letztesFeld is TextBox t) InTextBoxEinfuegen(t, zeichen);
        }

        private void InRichTextEinfuegen(RichTextBox box, string zeichen)
        {
            // Markierten Text zuerst entfernen -> danach ist die Auswahl leer.
            if (!box.Selection.IsEmpty)
                box.Selection.Text = "";

            // TextRange mit identischem Start und Ende = reine Einfügemarke.
            var marke = new TextRange(box.CaretPosition, box.CaretPosition);
            marke.Text = zeichen;

            box.CaretPosition = marke.End;
            box.Focus();
            WerkzeugeAktualisieren();
        }

        private void InTextBoxEinfuegen(TextBox feld, string zeichen)
        {
            int start = feld.SelectionStart;
            int laenge = feld.SelectionLength;

            // Remove/Insert statt SelectedText: SelectedText verhält sich bei
            // TwoWay-Bindungen mit UpdateSourceTrigger=PropertyChanged unzuverlässig,
            // weil die Bindung mitten in der Änderung zurückschreibt.
            string alt = feld.Text ?? "";
            feld.Text = alt.Remove(start, laenge).Insert(start, zeichen);

            // Cursor hinter das Zeichen. Muss NACH dem Setzen von .Text passieren,
            // weil das Neu-Setzen des Textes die Cursorposition zurückwirft.
            feld.CaretIndex = start + zeichen.Length;
            feld.Focus();
            WerkzeugeAktualisieren();
        }

        private void FormatUmschalten(Auszeichnung art)
        {
            var box = _aktiveBox;
            if (box == null) return;

            // Leeres Dokument -> selbst vormerken, NICHT den EditingCommand nutzen.
            if (DokumentOhneText(box))
            {
                switch (art)
                {
                    case Auszeichnung.Fett:
                        _vorFett = !(_vorFett ?? false); break;
                    case Auszeichnung.Kursiv:
                        _vorKursiv = !(_vorKursiv ?? false); break;
                    case Auszeichnung.Unterstrichen:
                        _vorUnterstrichen = !(_vorUnterstrichen ?? false); break;
                }
                WerkzeugeAktualisieren();
                return;
            }

            // Text vorhanden -> der reguläre Weg funktioniert zuverlässig.
            var befehl = art switch
            {
                Auszeichnung.Fett => EditingCommands.ToggleBold,
                Auszeichnung.Kursiv => EditingCommands.ToggleItalic,
                _ => EditingCommands.ToggleUnderline
            };

            if (befehl.CanExecute(null, box))
                befehl.Execute(null, box);

            WerkzeugeAktualisieren();
        }

        // Ein leeres Dokument liefert je nach Zustand "" oder "\r\n" – beides zählt als leer.
        private static bool DokumentOhneText(RichTextBox box) => new TextRange(box.Document.ContentStart, box.Document.ContentEnd).Text.Trim('\r', '\n').Length == 0;

        private void VormerkungLeeren()
        {
            _vorFett = null;
            _vorKursiv = null;
            _vorUnterstrichen = null;
        }

        // Absatz ohne sichtbaren Text. WICHTIG: über .Text prüfen, nicht über
        // TextRange.IsEmpty – letzteres meldet bereits bei einem leeren Run
        // "nicht leer", weil Start- und Endposition dann nicht identisch sind.


        // Öffnet den Übernahme-Dialog und kopiert den gewählten Text in diese Zeile.
        // Ist das Feld schon gefüllt, wird vorher gefragt: Ersetzen / Anhängen / Abbrechen.
        private void TextUebernehmen_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is not ZeileVM z)
                return;

            var dlg = new TextUebernahmeFenster
            {
                Owner = Window.GetWindow(this)   // liefert das MainWindow -> Dialog zentriert sich darauf
            };

            if (dlg.ShowDialog() != true || dlg.UebernommenerText == null)
                return;

            string neu = dlg.UebernommenerText;
            string neuFormatiert = dlg.UebernommenerTextFormatiert ?? "";   // NEU
            string vorhanden = z.EigenerText ?? "";

            // Feld leer -> direkt übernehmen, keine Rückfrage nötig.
            if (string.IsNullOrWhiteSpace(vorhanden))
            {
                TextInBoxSchreiben(z, neu, neuFormatiert, anhaengen: false);
                return;
            }

            // Feld hat bereits Inhalt -> nachfragen.
            // Ja = Ersetzen, Nein = Anhängen, Abbrechen = nichts tun.
            var antwort = MessageBox.Show(
                "Dieses Textfeld enthält bereits Text.\n\n" +
                "Ja = vorhandenen Text ersetzen\n" +
                "Nein = neuen Text anhängen\n" +
                "Abbrechen = nichts ändern",
                "Text übernehmen",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (antwort == MessageBoxResult.Yes)
                TextInBoxSchreiben(z, neu, neuFormatiert, anhaengen: false);
            else if (antwort == MessageBoxResult.No)
                TextInBoxSchreiben(z, neu, neuFormatiert, anhaengen: true);
            // Cancel -> bewusst nichts tun
        }

        // Schreibt in die RichTextBox der Zeile. Liegt der Quelltext formatiert
        // vor (Base64-XamlPackage), wird die Formatierung mit übernommen;
        // sonst wird der Klartext als einfacher Absatz eingefügt.
        private void TextInBoxSchreiben(ZeileVM z, string text, string formatiert, bool anhaengen)
        {
            if (!_boxen.TryGetValue(z, out var box))
            {
                // Kein sichtbares Feld (sollte nicht vorkommen) -> nur ins ViewModel.
                z.EigenerText = anhaengen
                    ? (z.EigenerText ?? "").TrimEnd() + "\n\n" + text
                    : text;
                z.EigenerTextFormatiert = "";
                return;
            }

            if (!anhaengen)
                box.Document.Blocks.Clear();
            else
                box.Document.Blocks.Add(new Paragraph());   // Leerzeile als Trenner

            if (!string.IsNullOrWhiteSpace(formatiert))
            {
                try
                {
                    // Erst in ein Zwischen-Dokument laden, dann die Blöcke umhängen.
                    // Direkt in das Ziel laden geht nicht zuverlässig, weil TextRange.Load
                    // beim Anhängen den vorhandenen Inhalt ersetzen würde.
                    var temp = new FlowDocument();
                    var bytes = Convert.FromBase64String(formatiert);
                    using (var ms = new MemoryStream(bytes))
                        new TextRange(temp.ContentStart, temp.ContentEnd)
                            .Load(ms, DataFormats.XamlPackage);

                    // ToList(), weil das Entfernen die Auflistung während der
                    // Schleife verändern würde. Ein Block gehört immer nur zu
                    // EINEM Dokument -> erst lösen, dann anhängen.
                    foreach (var block in temp.Blocks.ToList())
                    {
                        temp.Blocks.Remove(block);
                        box.Document.Blocks.Add(block);
                    }
                    SchriftVereinheitlichen(box.Document);
                    return;   // TextChanged schreibt bereits zurück
                }
                catch
                {
                    // Beschädigtes Paket -> unten auf Klartext zurückfallen.
                }
            }

            box.Document.Blocks.Add(new Paragraph(new Run(text)));
            // TextChanged schreibt automatisch ins ViewModel zurück.
        }

        // ═══════════════════════════════════════════════════════════
        // Bearbeiteten DB-Text in die Datenbank zurückspeichern
        // ═══════════════════════════════════════════════════════════

        private void BeschreibungSpeichern_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is not ZeileVM z) return;

            if (string.IsNullOrWhiteSpace(z.Definition) || string.IsNullOrWhiteSpace(z.AusgewaehlterName))
            {
                MessageBox.Show("Es sind keine Definition und kein Name gewählt.");
                return;
            }

            var dlg = new BeschreibungSpeichernFenster(
                z.Definition, z.AusgewaehlterName, _quelle.NamenFuer(z.Definition))
            {
                Owner = Window.GetWindow(this)
            };

            if (dlg.ShowDialog() != true || string.IsNullOrWhiteSpace(dlg.GewaehlterName))
                return;

            string name = dlg.GewaehlterName.Trim();
            string text = z.Baubeschreibung;   // Getter liefert den bearbeiteten Text

            try
            {
                InDatenbankSchreiben(AktiverBereich.Nummer, z.Definition, z.AusgewaehlterName, name, text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Speichern fehlgeschlagen:\n" + ex.Message,
                    "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Cache der Datenquelle nachziehen, damit der (neue) Name sofort auflösbar ist
            _quelle.EintragUebernehmen(z.Definition, name, text);

            // Alle Zeilen über die evtl. erweiterte Namensliste informieren.
            // Die bestehenden Auswahlen bleiben erhalten, weil nur Namen HINZUKOMMEN.
            foreach (var t in Titel)
                foreach (var u in t.Untertitel)
                    foreach (var zeile in u.Zeilen)
                        zeile.NamensListeAktualisieren();

            // Zeile auf den gespeicherten Namen umstellen -> Markierung verschwindet
            z.BearbeitungGespeichert(name);
        }

        // Schreibt den Text in die Bauteile-Tabelle. Existiert der Zielname schon,
        // wird sein Text überschrieben; sonst entsteht ein neuer Eintrag, der den
        // Bildnamen des Ursprungseintrags erbt. Das Original bleibt bei neuem
        // Namen unangetastet.
        // Schreibt den Text in die Bauteile-Tabelle des AKTIVEN Baubereichs.
        // Existiert der Zielname dort schon, wird sein Text überschrieben; sonst
        // entsteht ein neuer Eintrag, der den Bildnamen des Ursprungseintrags
        // erbt. Das Original bleibt bei neuem Namen unangetastet.
        private static void InDatenbankSchreiben(int bereich, string definition, string alterName, string neuerName, string text)
        {
            using var db = new AppDbContext();
            db.Database.EnsureCreated();

            // Der Bereichsfilter gehört in JEDE Abfrage: Ohne ihn würde ein
            // gleichnamiger Eintrag aus einem anderen LV überschrieben.
            var ziel = db.Bauteile.FirstOrDefault(
                b => b.Bereich == bereich && b.Definition == definition && b.Name == neuerName);

            if (ziel != null)
            {
                ziel.Baubeschreibung = text;
            }
            else
            {
                var quelle = db.Bauteile.FirstOrDefault(
                    b => b.Bereich == bereich && b.Definition == definition && b.Name == alterName);

                db.Bauteile.Add(new Bauteil
                {
                    Bereich = bereich,
                    Definition = definition,
                    Name = neuerName,
                    Baubeschreibung = text,
                    BildPfad = quelle?.BildPfad ?? ""
                });
            }

            // Änderungszeitpunkt vermerken (Info-Kachel der Startseite).
            // Läuft im selben Kontext -> ein gemeinsames SaveChanges.
            LvStempel.Markieren(db, bereich);

            db.SaveChanges();
        }

        // ═══════════════════════════════════════════════════════════
        // Savegame: Zustand lesen / anwenden (kompletter Titel-Baum)
        // ═══════════════════════════════════════════════════════════

        // Läuft den ganzen Baum durch und kopiert alle Werte in DTOs.
        public List<TitelDaten> ZustandLesen()
        {
            var liste = new List<TitelDaten>();

            foreach (var t in Titel)
            {
                var td = new TitelDaten
                {
                    Titel = t.Titel ?? "",
                    IstAktiv = t.IstAktiv
                };

                foreach (var u in t.Untertitel)
                {
                    var ud = new UntertitelDaten
                    {
                        IstAktiv = u.IstAktiv,
                        Definition = u.Definition ?? ""
                    };

                    foreach (var z in u.Zeilen)
                    {
                        ud.Zeilen.Add(new ZeileDaten
                        {
                            BildName = z.BildName ?? "",
                            AusgewaehlterName = z.AusgewaehlterName ?? "",
                            Baubeschreibung = z.Baubeschreibung ?? "",   // NEU: aufgelöster Text mitschreiben
                            EigenerText = z.EigenerText ?? "",
                            EigenerTextFormatiert = z.EigenerTextFormatiert ?? "",
                            IstTextEingabe = z.IstTextEingabe   // nur informativ, wird beim Laden abgeleitet
                        });
                    }

                    td.Untertitel.Add(ud);
                }

                liste.Add(td);
            }

            return liste;
        }

        // Baut den kompletten Baum aus den DTOs neu auf.
        public void ZustandAnwenden(List<TitelDaten> daten)
        {
            // Nichts gespeichert -> Standard-Titel aus den Optionen laden,
            // damit die Seite nicht leer bleibt.
            if (daten == null || daten.Count == 0)
            {
                TitelAusDatenbankLaden();
                return;
            }

            Titel.Clear();

            foreach (var td in daten)
            {
                var t = new TitelVM(_quelle, td.Titel ?? "");
                t.IstAktiv = td.IstAktiv;

                foreach (var ud in td.Untertitel)
                {
                    // Untertitel über die vorhandene Methode anlegen -> setzt 'Eltern' korrekt.
                    t.UntertitelHinzufuegen();
                    var u = t.Untertitel[t.Untertitel.Count - 1];

                    // WICHTIG: Definition VOR den Zeilen setzen, damit VerfuegbareNamen
                    // beim späteren Setzen von AusgewaehlterName bereits stimmt (siehe Erklärung).
                    u.Definition = string.IsNullOrEmpty(ud.Definition) ? null : ud.Definition;

                    foreach (var zd in ud.Zeilen)
                    {
                        u.ZeileHinzufuegen();               // -> setzt 'eltern' korrekt
                        var z = u.Zeilen[u.Zeilen.Count - 1];

                        z.BildName = string.IsNullOrEmpty(zd.BildName) ? null : zd.BildName;

                        // AusgewaehlterName steuert automatisch IstTextEingabe / IstAnzeige / Baubeschreibung.
                        // "Texteingabe" ist immer in VerfuegbareNamen enthalten und bleibt daher erhalten.
                        z.AusgewaehlterName =
                            string.IsNullOrEmpty(zd.AusgewaehlterName) ? null : zd.AusgewaehlterName;

                        // WICHTIG: NACH AusgewaehlterName setzen — dessen Setter leert den
                        // Rückfalltext bewusst. Kann die lokale DB den Namen auflösen, bleibt
                        // dieser Wert unsichtbar in Reserve; wenn nicht, zeigt Spalte E ihn an.
                        z.GespeicherterText = zd.Baubeschreibung ?? "";

                        // NEU: Weicht der im Projekt gespeicherte Text vom aktuell aufgelösten
                        // Text ab, war er zum Speicherzeitpunkt bearbeitet -> Bearbeitung
                        // wiederherstellen. Die Zeile erscheint dann wieder als "geändert".
                        // (Greift NICHT im Rückfall-Fall: dort liefert der Getter bereits den
                        // GespeicherterText, beide Werte sind gleich, der Setter tut nichts.)
                        if (!z.IstTextEingabe &&
                            !string.IsNullOrEmpty(zd.Baubeschreibung) &&
                            zd.Baubeschreibung != z.Baubeschreibung)
                        {
                            z.Baubeschreibung = zd.Baubeschreibung;
                        }

                        // EigenerText immer setzen – wird nur im Texteingabe-Modus angezeigt.
                        z.EigenerText = zd.EigenerText ?? "";
                        z.EigenerTextFormatiert = zd.EigenerTextFormatiert ?? "";
                    }

                    // IstAktiv am Schluss – Reihenfolge hier unkritisch.
                    u.IstAktiv = ud.IstAktiv;
                }

                Titel.Add(t);   // erst ganz zum Schluss anhängen -> kein Zwischen-Rendern
            }
        }

        // ── "Neu": Titel-Baum auf die Standard-Titel aus den Optionen zurücksetzen ──
        public void Zuruecksetzen()
        {
            TitelAusDatenbankLaden();
        }

        // ═══════════════════════════════════════════════════════════
        // Druck: FlowDocument aufbauen
        // ═══════════════════════════════════════════════════════════

        // Die Werte liegen jetzt zentral in Drucklayout, weil die Einleitung sie
        // ebenfalls braucht. Die bisherigen Namen bleiben als Verweise stehen –
        // dadurch ist im übrigen Code dieser Datei keine Zeile anzupassen.
        private const double DruckBalkenEinzugRechts = Drucklayout.BalkenEinzugRechts;
        private const double DruckTextEinzug = Drucklayout.TextEinzug;
        private const double DruckBildBreite = Drucklayout.BildBreite;
        private const double DruckAbstandTitel = Drucklayout.AbstandTitel;
        private const double DruckAbstandUntertitel = Drucklayout.AbstandUntertitel;
        private const double DruckAbstandZeile = Drucklayout.AbstandZeile;

        private static readonly Color DruckFarbeKopf = Drucklayout.FarbeKopf;
        private static readonly Color DruckFarbeTitel = Drucklayout.FarbeTitel;
        private static readonly Color DruckFarbeUntertitel = Drucklayout.FarbeUntertitel;

        private const double DruckBildMaxHoehe = Drucklayout.BildMaxHoehe;
        private const double DruckBildAbstand = Drucklayout.BildAbstand;
        private const double DruckBreitenReserve = Drucklayout.BreitenReserve;

        /// <summary>
        /// Baut die komplette Baubeschreibung als FlowDocument auf.
        ///
        /// Bewusst FlowDocument und nicht FixedDocument: Nur so paginiert WPF selbst.
        /// Die Seitengeometrie wird hier NICHT gesetzt – darum kümmern sich
        /// PrintHelper.PreparePages für die Vorschau und GeometrieFuerDruck für den
        /// Ausdruck.
        /// </summary>
        /// <param name="projektName">Erscheint im obersten Balken.</param>
        /// <param name="mitKopfbalken">
        /// false, wenn das Dokument später an ein anderes angehängt wird – dann steht
        /// der Kopfbalken bereits dort.
        /// </param>
        /// <param name="vorspann">
        /// Blöcke, die zwischen Kopfbalken und erstem Titel eingefügt werden (die
        /// Einleitung). Ist etwas gesetzt, beginnt der erste Titel zwingend auf einer
        /// neuen Seite.
        ///
        /// Bewusst als Parameter statt als nachträgliches Umhängen: Die Blöcke werden
        /// so von Anfang an Teil DIESES Dokuments und erben dessen Schrift, Absatzstil
        /// und Textausrichtung. Beim Umhängen zwischen zwei fertigen Dokumenten ginge
        /// die Resources-Ebene verloren.
        /// </param>
        public FlowDocument BuildPrintDocument(string projektName = "", bool mitKopfbalken = true, IEnumerable<Block> vorspann = null)
        {
            var doc = new FlowDocument
            {
                FontFamily = (FontFamily)FindResource("FeldSchrift"),
                FontSize = (double)FindResource("FeldSchriftGroesse"),
                Foreground = Brushes.Black,

                // FlowDocument steht standardmäßig auf Blocksatz, die TextBox in der App
                // dagegen auf linksbündig. Das erzeugt nicht nur ein anderes Schriftbild,
                // sondern auch einen echten Fehler: Ein LineBreak in einem Blocksatzabsatz
                // zieht die vorangehende Zeile auf die volle Spaltenbreite auseinander.
                TextAlignment = TextAlignment.Left
            };

            // Absätze haben in WPF standardmäßig einen großzügigen Außenabstand.
            // Der wird global auf null gesetzt; alle Abstände kommen gezielt über
            // die Margin-Angaben weiter unten. Lokal gesetzte Margins schlagen diesen
            // Stil, weil ein lokaler Wert in der WPF-Wertepriorität über einem
            // Style-Setter steht.
            var absatzStil = new Style(typeof(Paragraph));
            absatzStil.Setters.Add(new Setter(Block.MarginProperty, new Thickness(0)));
            absatzStil.Setters.Add(new Setter(Block.TextAlignmentProperty, TextAlignment.Left));
            doc.Resources[typeof(Paragraph)] = absatzStil;

            var bilder = new BildCache();

            if (mitKopfbalken)
            {
                doc.Blocks.Add(BalkenAbsatz(
                    "Baubeschreibung - Bauvorhaben:   " + projektName,
                    DruckFarbeKopf, 16, 0, 0, 0, DruckAbstandTitel));
            }

            // Vorspann (Einleitung) direkt unter den Kopfbalken.
            bool mitVorspann = false;
            if (vorspann != null)
            {
                foreach (Block b in vorspann)
                {
                    doc.Blocks.Add(b);
                    mitVorspann = true;
                }
            }

            // Der erste Titel muss nach einem Vorspann auf eine neue Seite. Sonst
            // liefe die Baubeschreibung direkt unter dem Einleitungstext weiter.
            bool ersterTitelSteht = false;

            int titelIndex = 0;

            foreach (var t in Titel)
            {
                titelIndex++;

                if (!t.IstAktiv)
                    continue;

                // Jeder Untertitel wird zu einer eigenen Section. Die Section stellt selbst
                // nichts dar und ändert das Layout nicht - sie dient allein als Klammer,
                // damit die Umbruchoptimierung Anfang und Ende der Gruppe kennt.
                var gruppen = new List<Section>();
                string nummer = TitelNummer(t, titelIndex);
                int lfdUntertitel = 0;

                foreach (var u in t.Untertitel)
                {
                    if (!u.IstAktiv)
                        continue;   // Zum Stabilhalten der Nummern: diese Zeile nach
                                    // die folgende lfdUntertitel++-Zeile verschieben.

                    lfdUntertitel++;

                    var zeilenBloecke = new List<Block>();

                    foreach (var z in u.Zeilen)
                    {
                        Section abschnitt = ZeilenAbschnitt(z, bilder);
                        if (abschnitt != null)
                            zeilenBloecke.Add(abschnitt);
                    }

                    // Untertitel ohne jeden Text überspringen – ein einzelner Balken ohne
                    // Inhalt sieht im Ausdruck nach einem Fehler aus.
                    if (zeilenBloecke.Count == 0)
                        continue;

                    string beschriftung = $"{nummer}.{lfdUntertitel} {u.Definition ?? ""}".TrimEnd();

                    var gruppe = new Section();
                    gruppe.Blocks.Add(BalkenAbsatz(
                        beschriftung, DruckFarbeUntertitel, 14,
                        0, DruckBalkenEinzugRechts,
                        DruckAbstandUntertitel, DruckAbstandZeile));

                    foreach (Block b in zeilenBloecke)
                        gruppe.Blocks.Add(b);

                    gruppen.Add(gruppe);
                }

                if (gruppen.Count == 0)
                    continue;

                Paragraph titelBalken = BalkenAbsatz(
                    t.Titel ?? "", DruckFarbeTitel, 16,
                    0, 0, DruckAbstandTitel, DruckAbstandUntertitel);

                // Nur der ERSTE tatsächlich ausgegebene Titel bekommt den festen
                // Umbruch. Welcher das ist, steht erst hier fest: Titel ohne Inhalt
                // wurden oben übersprungen.
                bool festerUmbruch = mitVorspann && !ersterTitelSteht;
                if (festerUmbruch)
                    titelBalken.BreakPageBefore = true;

                ersterTitelSteht = true;

                doc.Blocks.Add(titelBalken);

                for (int i = 0; i < gruppen.Count; i++)
                {
                    // Beim ERSTEN Untertitel eines Titels wird der Umbruch vor den
                    // Titelbalken gesetzt, damit dieser mitwandert. Sonst bliebe "4."
                    // allein am Seitenende stehen und "4.1" begänne auf der nächsten Seite.
                    gruppen[i].Tag = new PrintHelper.Gruppenmarke
                    {
                        Umbruchziel = (i == 0) ? (Block)titelBalken : gruppen[i],

                        // Schützt den Umbruch vor dem Zurücksetzen in
                        // SeitenumbruecheOptimieren – der läuft bei Vorschau und
                        // Ausdruck jeweils erneut.
                        FesterUmbruch = (i == 0) && festerUmbruch
                    };

                    doc.Blocks.Add(gruppen[i]);
                }
            }

            return doc;
        }

        /// <summary>
        /// Ermittelt die führende Nummer eines Titels, z. B. "2" aus "2. Rohbau".
        /// Fällt auf die Position im Baum zurück, falls in den Optionen etwas
        /// anderes als eine Zahl vor dem Punkt steht.
        /// </summary>
        private static string TitelNummer(TitelVM t, int position)
        {
            string titel = t.Titel ?? "";
            int punkt = titel.IndexOf('.');

            if (punkt > 0)
            {
                string nr = titel.Substring(0, punkt).Trim();
                if (nr.Length > 0)
                    return nr;
            }

            return position.ToString();
        }

        /// <summary>
        /// Baut eine einzelne Zeile (Text plus optionales Bild) als zusammenhängenden
        /// Abschnitt. Gibt null zurück, wenn die Zeile keinen druckbaren Inhalt hat.
        /// </summary>
        /// <summary>
        /// Baut eine einzelne Zeile (Text plus optionales Bild) als zusammenhängenden
        /// Abschnitt. Gibt null zurück, wenn die Zeile keinen druckbaren Inhalt hat.
        /// </summary>
        /// <summary>
        /// Baut eine einzelne Zeile (Text plus optionales Bild) als zusammenhängenden
        /// Abschnitt. Gibt null zurück, wenn die Zeile keinen druckbaren Inhalt hat.
        /// </summary>
        private Section ZeilenAbschnitt(ZeileVM z, BildCache bilder)
        {
            List<Block> bloecke = TextBloecke(z);
            ImageSource bild = bilder.Holen(z.BildName);

            if (bloecke.Count == 0 && bild == null)
                return null;

            var abschnitt = new Section
            {
                Margin = new Thickness(DruckTextEinzug, 0, 0, DruckAbstandZeile)
            };

            if (bild != null)
            {
                // Zweispaltige Tabelle statt Figure. Eine Figure paginiert eigenständig:
                // Passt sie nicht mehr auf die Seite, wandert NUR SIE weiter, während
                // ihr Text zurückbleibt - genau das war bei "3.1 Aufzüge" und beim
                // Balkonbild zu sehen. KeepWithNext greift dort nicht, weil eine Figure
                // außerhalb des normalen Blockflusses liegt.
                //
                // In einer Tabellenzeile stehen Text und Bild dagegen fest nebeneinander
                // und können sich nicht mehr trennen.
                abschnitt.Blocks.Add(ZeileMitBild(bloecke, bild));
            }
            else
            {
                ZusammenhaltSetzen(bloecke);
                foreach (Block b in bloecke)
                    abschnitt.Blocks.Add(b);
            }

            return abschnitt;
        }

        /// <summary>
        /// Setzt Text und Bild nebeneinander: links der Text, rechts das Bild.
        /// Beide Spaltenbreiten werden ausgerechnet und absolut gesetzt.
        /// </summary>
        private static Table ZeileMitBild(List<Block> textBloecke, ImageSource bild)
        {
            BildMasse(bild, out double bildBreite, out double bildHoehe);

            // Verfügbare Breite ausrechnen statt Star verwenden.
            //
            // Star funktioniert hier NICHT: Eine Table im FlowDocument verteilt
            // Star-Anteile auf die Breite, die ihr der Container zuweist. Beim
            // Ausmessen bekommt der Block aber zunächst unbegrenzte Breite angeboten,
            // und ein Star-Anteil von "unendlich" ist nicht auflösbar. WPF fällt dann
            // auf den Mindestbedarf zurück - bei Text sind das genau ein Zeichen
            // Breite. Die Pixelspalte daneben bleibt unbeeinflusst, weil sie ihr Maß
            // fest mitbringt.
            double verfuegbar = PrintHelper.A4_W
                              - 2 * PrintHelper.Margin      // Seitenrand links und rechts
                              - DruckTextEinzug             // Einzug der Section
                              - DruckBreitenReserve;        // Puffer Vorschau/Ausdruck

            double spalteBild = bildBreite + DruckBildAbstand;
            double spalteText = verfuegbar - spalteBild;

            // Notbremse: Bliebe für den Text weniger als ein Drittel der Breite,
            // wird lieber das Bild verkleinert. Sonst entstünde wieder eine
            // Ein-Zeichen-Spalte, nur auf anderem Weg.
            double mindestText = verfuegbar / 3.0;
            if (spalteText < mindestText)
            {
                spalteText = mindestText;
                spalteBild = verfuegbar - spalteText;

                double neueBildBreite = spalteBild - DruckBildAbstand;
                if (neueBildBreite > 0)
                {
                    // Höhe proportional mitziehen, sonst wird das Bild verzerrt.
                    bildHoehe = bildHoehe * neueBildBreite / bildBreite;
                    bildBreite = neueBildBreite;
                }
            }

            var tabelle = new Table
            {
                CellSpacing = 0,
                Margin = new Thickness(0)
            };

            tabelle.Columns.Add(new TableColumn { Width = new GridLength(spalteText) });
            tabelle.Columns.Add(new TableColumn { Width = new GridLength(spalteBild) });

            var zelleText = new TableCell { Padding = new Thickness(0, 0, DruckBildAbstand, 0) };
            foreach (Block b in textBloecke)
                zelleText.Blocks.Add(b);

            var bildElement = new Image
            {
                Source = bild,
                Width = bildBreite,
                Height = bildHoehe,
                // Fill statt Uniform: Das Seitenverhältnis ist bereits exakt berechnet.
                // Uniform ließe bei Rundungsdifferenzen einen Leerstreifen.
                Stretch = Stretch.Fill
            };

            // BlockUIContainer ist der einzige Weg, ein WPF-Steuerelement in ein
            // FlowDocument zu setzen. Margin auf null, sonst kommt der Standardabstand
            // des Containers zum Zellenabstand hinzu.
            var zelleBild = new TableCell(new BlockUIContainer(bildElement)
            {
                Margin = new Thickness(0)
            })
            {
                Padding = new Thickness(0)
            };

            var zeile = new TableRow();
            zeile.Cells.Add(zelleText);
            zeile.Cells.Add(zelleBild);

            var gruppe = new TableRowGroup();
            gruppe.Rows.Add(zeile);
            tabelle.RowGroups.Add(gruppe);

            return tabelle;
        }

        /// <summary>
        /// Rechnet Breite und Höhe eines Bildes aus. Ausgangspunkt ist die
        /// Zielbreite; überschreitet die daraus folgende Höhe das erlaubte Maß,
        /// wird stattdessen über die Höhe skaliert.
        /// </summary>
        private static void BildMasse(ImageSource bild, out double breite, out double hoehe)
        {
            breite = DruckBildBreite;

            // PixelWidth/PixelHeight statt Width/Height: Letztere rechnen die DPI-Angabe
            // aus der Bilddatei ein, die in JPEGs oft falsch gesetzt ist. Für das
            // Seitenverhältnis sind die reinen Pixelmaße verlässlicher.
            if (bild is not BitmapSource bs || bs.PixelWidth <= 0 || bs.PixelHeight <= 0)
            {
                hoehe = breite;
                return;
            }

            double verhaeltnis = bs.PixelHeight / (double)bs.PixelWidth;
            hoehe = breite * verhaeltnis;

            // Hochformat begrenzen, sonst füllt ein einzelnes Foto den halben Satzspiegel.
            if (hoehe > DruckBildMaxHoehe)
            {
                hoehe = DruckBildMaxHoehe;
                breite = hoehe / verhaeltnis;
            }
        }

        /// <summary>
        /// Kettet die Absätze einer Zeile aneinander, damit Bild und Text gemeinsam
        /// auf eine Seite wandern.
        /// </summary>
        private static void ZusammenhaltSetzen(List<Block> bloecke)
        {
            var absaetze = new List<Paragraph>();
            foreach (Block b in bloecke)
                AbsaetzeSammeln(b, absaetze);

            for (int i = 0; i < absaetze.Count; i++)
            {
                // Der Absatz selbst wird nicht über den Seitenrand hinweg aufgeteilt.
                absaetze[i].KeepTogether = true;

                // ... und bleibt beim nachfolgenden Block. Der LETZTE bekommt das
                // bewusst nicht: Sonst zöge er den nächsten Abschnitt mit hinüber und
                // die Kette liefe über die ganze Seite weiter.
                absaetze[i].KeepWithNext = i < absaetze.Count - 1;
            }
        }

        /// <summary>
        /// Sammelt alle Absätze eines Blocks ein – auch solche, die in einer
        /// Aufzählung stecken.
        /// </summary>
        private static void AbsaetzeSammeln(Block block, List<Paragraph> ziel)
        {
            switch (block)
            {
                case Paragraph p:
                    ziel.Add(p);
                    break;

                case List liste:
                    // Eine List ist selbst ein Block ohne KeepTogether. Die eigentlichen
                    // Absätze liegen in den ListItems und müssen dort gesetzt werden -
                    // sonst reißt eine Aufzählung mitten im Bild auseinander.
                    foreach (ListItem eintrag in liste.ListItems)
                        foreach (Block b in eintrag.Blocks)
                            AbsaetzeSammeln(b, ziel);
                    break;

                case Section s:
                    foreach (Block b in s.Blocks)
                        AbsaetzeSammeln(b, ziel);
                    break;

                // BlockUIContainer und Table bleiben unberührt - beide haben keine
                // entsprechende Eigenschaft.

                case Table tabelle:
                    // Absätze in Tabellenzellen mitnehmen. Der Zusammenhalt einer
                    // Tabellenzeile lässt sich damit nicht erzwingen, aber innerhalb einer
                    // Zelle bleiben zusammengehörige Absätze beieinander.
                    foreach (TableRowGroup gruppe in tabelle.RowGroups)
                        foreach (TableRow zeile in gruppe.Rows)
                            foreach (TableCell zelle in zeile.Cells)
                                foreach (Block b in zelle.Blocks)
                                    AbsaetzeSammeln(b, ziel);
                    break;
            }
        }

        /// <summary>
        /// Liefert den Inhalt der Spalte E als FlowDocument-Blöcke – je nach Modus aus
        /// dem formatierten Eigentext oder aus dem aufgelösten Datenbanktext.
        /// </summary>
        private static List<Block> TextBloecke(ZeileVM z)
        {
            var ergebnis = new List<Block>();

            if (z.IstTextEingabe)
            {
                // Formatierter Text liegt als Base64-XamlPackage vor. Erst in ein
                // Zwischendokument laden, dann die Blöcke umhängen: Ein Block gehört
                // immer nur zu EINEM Dokument und muss vor dem Anhängen gelöst werden.
                if (!string.IsNullOrWhiteSpace(z.EigenerTextFormatiert))
                {
                    try
                    {
                        var temp = new FlowDocument();
                        byte[] bytes = Convert.FromBase64String(z.EigenerTextFormatiert);
                        using (var ms = new MemoryStream(bytes))
                        {
                            new TextRange(temp.ContentStart, temp.ContentEnd)
                                .Load(ms, DataFormats.XamlPackage);
                        }

                        foreach (Block b in temp.Blocks.ToList())
                        {
                            temp.Blocks.Remove(b);
                            ergebnis.Add(b);
                        }

                        if (ergebnis.Count > 0)
                            return ergebnis;
                    }
                    catch
                    {
                        // Beschädigtes Paket -> unten auf den Klartext zurückfallen.
                        ergebnis.Clear();
                    }
                }

                if (!string.IsNullOrWhiteSpace(z.EigenerText))
                    ergebnis.Add(TextAbsatz(z.EigenerText));

                return ergebnis;
            }

            // Anzeige-Modus: aufgelöster Datenbanktext, gegebenenfalls die Bearbeitung.
            string text = z.Baubeschreibung;
            if (!string.IsNullOrWhiteSpace(text))
                ergebnis.Add(TextAbsatz(text));

            return ergebnis;
        }

        /// <summary>
        /// Wandelt einen mehrzeiligen Klartext in EINEN Absatz mit Zeilenumbrüchen um.
        /// </summary>
        private static Paragraph TextAbsatz(string text)
        {
            var p = new Paragraph();

            // Zwingend: Ein Run mit eingebettetem "\n" erzeugt im FlowDocument KEINEN
            // Zeilenumbruch, das Zeichen wird schlicht ignoriert. Die Zeilen des
            // Eingabefelds gingen damit im Ausdruck verloren.
            //
            // LineBreak statt eines eigenen Absatzes je Zeile: Zusammengehörige Zeilen
            // bleiben so ein Block und bekommen keinen Absatzabstand dazwischen –
            // genau wie in der Excel-Vorlage.
            string aufbereitet = (text ?? "")
                .Replace("\r\n", "\n")
                .Replace('\r', '\n');

            // Tabulatoren entschärfen. In alten LV-Texten stecken Tabs, die in der App
            // wie ein Zeilenumbruch WIRKEN: Der Sprung zum nächsten Tabstopp geht dort
            // über die Feldbreite hinaus, also bricht die TextBox um. Im Ausdruck ist
            // die Spalte breiter, der Tabstopp passt noch in die Zeile - und es
            // entsteht eine Lücke mitten im Satz statt eines Umbruchs.
            //
            // Ein einzelnes Leerzeichen wäre die neutrale Wahl, würde hier aber
            // Überschrift und Fließtext in EINE Zeile ziehen. Der Umbruch entspricht
            // dem, was der Anwender in der App sieht.
            aufbereitet = aufbereitet.Replace("\t", "\n");

            string[] zeilen = aufbereitet.Split('\n');

            for (int i = 0; i < zeilen.Length; i++)
            {
                if (i > 0) p.Inlines.Add(new LineBreak());
                p.Inlines.Add(new Run(zeilen[i]));
            }

            return p;
        }

        /// <summary>
        /// Erzeugt einen farbigen Balken als FlowDocument-Absatz.
        /// Die Umsetzung liegt in Drucklayout, damit Einleitung und Eingabe
        /// zwingend identische Balken erzeugen.
        /// </summary>
        private static Paragraph BalkenAbsatz(string text, Color farbe, double schriftgroesse, double einzugLinks, double einzugRechts, double abstandOben, double abstandUnten)
            => Drucklayout.BalkenAbsatz(text, farbe, schriftgroesse,
                                        einzugLinks, einzugRechts, abstandOben, abstandUnten);

        /// <summary>
        /// Lädt Bilder auf Anforderung aus der Datenbank und hält sie für die Dauer
        /// eines Druckvorgangs vor. Ohne Zwischenspeicher würde dasselbe Bild bei
        /// mehrfacher Verwendung mehrfach gelesen und dekodiert.
        /// </summary>
        private sealed class BildCache
        {
            private readonly Dictionary<string, ImageSource> _cache = new();

            public ImageSource Holen(string bildName)
            {
                if (string.IsNullOrWhiteSpace(bildName))
                    return null;

                // Ein bereits als null gemerkter Eintrag bedeutet: Bild existiert nicht.
                // Die Abfrage wird dann kein zweites Mal gestellt.
                if (_cache.TryGetValue(bildName, out var vorhanden))
                    return vorhanden;

                ImageSource quelle = null;
                try
                {
                    using var db = new AppDbContext();

                    // Nur die Bytes selektieren – die übrigen Spalten der Entität
                    // werden hier nicht gebraucht.
                    byte[] bytes = db.Bilder
                                     .Where(b => b.BildName == bildName)
                                     .Select(b => b.BildDaten)
                                     .FirstOrDefault();

                    if (bytes != null && bytes.Length > 0)
                    {
                        var bmp = new BitmapImage();
                        using var ms = new MemoryStream(bytes);
                        bmp.BeginInit();
                        // OnLoad ist zwingend: Sonst hält das BitmapImage den Stream
                        // offen, der beim Verlassen des using-Blocks verschwindet.
                        bmp.CacheOption = BitmapCacheOption.OnLoad;
                        bmp.StreamSource = ms;
                        bmp.EndInit();
                        // Freeze macht das Bild threadsicher und spart beim Rendern
                        // eine interne Kopie.
                        bmp.Freeze();
                        quelle = bmp;
                    }
                }
                catch
                {
                    // Ein fehlendes oder beschädigtes Bild darf den Druck nicht abbrechen.
                    quelle = null;
                }

                _cache[bildName] = quelle;
                return quelle;
            }
        }
    }
}