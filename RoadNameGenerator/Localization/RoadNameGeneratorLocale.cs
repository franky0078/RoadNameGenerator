using System.Collections.Generic;
using RoadNameGenerator.Naming;

namespace RoadNameGenerator.Localization
{
    internal static class RoadNameGeneratorLocale
    {
        public static IEnumerable<KeyValuePair<string, string>> AddEntries(
            Dictionary<string, string> entries, Setting setting, string language)
        {
            entries[setting.GetOptionLabelLocaleID(nameof(Setting.NamingStyleSelection))] =
                entries[setting.GetOptionLabelLocaleID(nameof(Setting.SelectedNamingStyle))];
            entries[setting.GetOptionDescLocaleID(nameof(Setting.NamingStyleSelection))] =
                entries[setting.GetOptionDescLocaleID(nameof(Setting.SelectedNamingStyle))];
            string[] text;
            switch (language)
            {
                default:
                    text = new[]
                    {
                        "Custom road names", // CustomNamesGroup
                        "Individual roads", // SingleRoadGroup
                        "Name source", // CustomNamesLabel
                        "Choose the selected naming style, only your custom list, or a 50/50 mix while custom names are available. In custom-only mode, roads keep their current names when the list is empty or exhausted.", // CustomNamesDesc
                        "Open name list folder", // OpenFolderLabel
                        "Opens ModsData/RoadNameGenerator. Edit CustomRoadNames.txt as UTF-8: one complete road name per line. Empty lines and # comments are ignored. Names longer than 128 characters are ignored.", // OpenFolderDesc
                        "Reload custom name list", // ReloadLabel
                        "Reloads CustomRoadNames.txt without restarting the game. Duplicate entries are ignored. Existing roads are not renamed by reloading.", // ReloadDesc
                        "Show road name button", // ShowButtonLabel
                        "Shows a toolbar button for a movable window with suggestions for the selected road. Click a suggestion to apply it.", // ShowButtonDesc
                        "Selected naming style", // Disabled
                        "Only custom names", // CustomOnly
                        "Mix custom names and naming style", // Mixed
                        "Road name suggestions", // Toggle
                        "Road Name Generator", // Title
                        "Current road name", // CurrentName
                        "Name suggestions", // Suggestions
                        "Apply", // Apply
                        "Apply this name to the selected road.", // ApplyHint
                        "New suggestions", // Refresh
                        "No unused names are available. Add more names to your custom list or select another name source in the mod options.", // NoSuggestions
                        "Select a road using the game’s information tool. The suggestions apply to the entire named road, including its connected segments.", // SelectRoad
                        "Loaded custom names", // CustomList
                        "CustomRoadNames.txt: one complete name per line. Select the name source in the dropdown above.", // ListHint
                        "Open folder", // OpenFolder
                        "Reload list", // ReloadList
                        "Could not read the list. Check the file, UTF-8 encoding and access permissions. The last successfully loaded list is retained.", // ListError
                        "Road renamed.", // Applied
                        "The road or name is no longer available. Suggestions have been refreshed.", // Unavailable
                        "The selection or settings changed. Suggestions have been refreshed.", // Stale
                        "No list loaded", // NoList
                        "Naming style", // NamingStyle
                        "Choose a naming style or your custom list here. The selection is saved in the mod options and suggestions are refreshed.", // NamingStyleHint
                        "Custom name list", // CustomListSource
                    };
                    break;
                case "de":
                    text = new[]
                    {
                        "Eigene Straßennamen", // CustomNamesGroup
                        "Einzelne Straßen", // SingleRoadGroup
                        "Namensquelle", // CustomNamesLabel
                        "Verwendet den gewählten Namensstil, nur die eigene Liste oder eine 50/50-Mischung, solange eigene Namen verfügbar sind. Bei „Nur eigene Namen“ bleiben die bisherigen Namen erhalten, wenn die Liste leer oder aufgebraucht ist.", // CustomNamesDesc
                        "Ordner der Namensliste öffnen", // OpenFolderLabel
                        "Öffnet ModsData/RoadNameGenerator. CustomRoadNames.txt als UTF-8 bearbeiten: ein vollständiger Straßenname pro Zeile. Leerzeilen und #-Kommentare werden ignoriert. Namen mit mehr als 128 Zeichen werden ignoriert.", // OpenFolderDesc
                        "Eigene Namensliste neu laden", // ReloadLabel
                        "Lädt CustomRoadNames.txt ohne Spielneustart neu. Doppelte Einträge werden ignoriert. Vorhandene Straßen werden durch das Neuladen nicht umbenannt.", // ReloadDesc
                        "Button für Straßennamen anzeigen", // ShowButtonLabel
                        "Zeigt einen Toolbar-Button für ein verschiebbares Fenster mit Vorschlägen zur ausgewählten Straße. Ein Klick auf einen Vorschlag übernimmt den Namen.", // ShowButtonDesc
                        "Gewählter Namensstil", // Disabled
                        "Nur eigene Namen", // CustomOnly
                        "Eigene Namen und Namensstil mischen", // Mixed
                        "Namensvorschläge für Straßen", // Toggle
                        "Road Name Generator", // Title
                        "Aktueller Straßenname", // CurrentName
                        "Namensvorschläge", // Suggestions
                        "Übernehmen", // Apply
                        "Diesen Namen für die ausgewählte Straße übernehmen.", // ApplyHint
                        "Neue Vorschläge", // Refresh
                        "Keine ungenutzten Namen verfügbar. Ergänze die eigene Namensliste oder wähle eine andere Namensquelle in den Modoptionen.", // NoSuggestions
                        "Wähle eine Straße mit dem Informationswerkzeug des Spiels aus. Die Vorschläge gelten für die gesamte benannte Straße einschließlich ihrer verbundenen Abschnitte.", // SelectRoad
                        "Geladene eigene Namen", // CustomList
                        "CustomRoadNames.txt: ein vollständiger Name pro Zeile. Die Namensquelle wählst du im Dropdown oben.", // ListHint
                        "Ordner öffnen", // OpenFolder
                        "Liste neu laden", // ReloadList
                        "Die Liste konnte nicht gelesen werden. Datei, UTF-8-Kodierung und Zugriffsrechte prüfen. Die zuletzt erfolgreich geladene Liste bleibt erhalten.", // ListError
                        "Straße umbenannt.", // Applied
                        "Die Straße oder der Name ist nicht mehr verfügbar. Die Vorschläge wurden aktualisiert.", // Unavailable
                        "Auswahl oder Einstellungen geändert. Die Vorschläge wurden aktualisiert.", // Stale
                        "Keine Liste geladen", // NoList
                        "Namensstil", // NamingStyle
                        "Wähle hier einen Namensstil oder deine eigene Liste. Die Auswahl wird in den Modoptionen gespeichert und die Vorschläge werden aktualisiert.", // NamingStyleHint
                        "Eigene Namensliste", // CustomListSource
                    };
                    break;
                case "es":
                    text = new[]
                    {
                        "Nombres de calles personalizados", // CustomNamesGroup
                        "Calles individuales", // SingleRoadGroup
                        "Origen de los nombres", // CustomNamesLabel
                        "Usa el estilo elegido, solo tu lista o una mezcla al 50 % mientras queden nombres personalizados. Con solo nombres personalizados, las calles conservan su nombre si la lista está vacía o agotada.", // CustomNamesDesc
                        "Abrir carpeta de nombres", // OpenFolderLabel
                        "Abre ModsData/RoadNameGenerator. Edita CustomRoadNames.txt en UTF-8: un nombre completo por línea. Se ignoran líneas vacías, comentarios # y nombres de más de 128 caracteres.", // OpenFolderDesc
                        "Recargar lista personalizada", // ReloadLabel
                        "Recarga CustomRoadNames.txt sin reiniciar el juego. Se ignoran duplicados. Recargar no cambia los nombres de las calles existentes.", // ReloadDesc
                        "Mostrar botón de nombres", // ShowButtonLabel
                        "Muestra un botón para una ventana móvil con sugerencias para la calle seleccionada. Haz clic en una sugerencia para aplicarla.", // ShowButtonDesc
                        "Estilo seleccionado", // Disabled
                        "Solo nombres personalizados", // CustomOnly
                        "Mezclar lista y estilo", // Mixed
                        "Sugerencias de nombres", // Toggle
                        "Road Name Generator", // Title
                        "Nombre actual", // CurrentName
                        "Nombres sugeridos", // Suggestions
                        "Aplicar", // Apply
                        "Aplicar este nombre a la calle seleccionada.", // ApplyHint
                        "Nuevas sugerencias", // Refresh
                        "No quedan nombres disponibles. Añade nombres a tu lista o cambia el origen en las opciones del mod.", // NoSuggestions
                        "Selecciona una calle con la herramienta de información del juego. Las sugerencias se aplican a toda la calle y sus tramos conectados.", // SelectRoad
                        "Nombres personalizados cargados", // CustomList
                        "CustomRoadNames.txt: un nombre completo por línea. Elige el origen en el desplegable de arriba.", // ListHint
                        "Abrir carpeta", // OpenFolder
                        "Recargar lista", // ReloadList
                        "No se pudo leer la lista. Revisa el archivo, UTF-8 y los permisos. Se conserva la última lista cargada correctamente.", // ListError
                        "Nombre de calle cambiado.", // Applied
                        "La calle o el nombre ya no está disponible. Se actualizaron las sugerencias.", // Unavailable
                        "La selección o los ajustes cambiaron. Se actualizaron las sugerencias.", // Stale
                        "Ninguna lista cargada", // NoList
                        "Estilo de nombres", // NamingStyle
                        "Elige un estilo o tu lista. La selección se guarda en las opciones del mod y actualiza las sugerencias.", // NamingStyleHint
                        "Lista de nombres personalizados", // CustomListSource
                    };
                    break;
                case "fr":
                    text = new[]
                    {
                        "Noms de rues personnalisés", // CustomNamesGroup
                        "Rues individuelles", // SingleRoadGroup
                        "Source des noms", // CustomNamesLabel
                        "Utilise le style choisi, uniquement votre liste ou un mélange à 50/50 tant que des noms personnalisés restent disponibles. En mode personnalisé uniquement, les noms actuels sont conservés si la liste est vide ou épuisée.", // CustomNamesDesc
                        "Ouvrir le dossier des noms", // OpenFolderLabel
                        "Ouvre ModsData/RoadNameGenerator. Modifiez CustomRoadNames.txt en UTF-8 : un nom complet par ligne. Les lignes vides, commentaires # et noms de plus de 128 caractères sont ignorés.", // OpenFolderDesc
                        "Recharger la liste personnalisée", // ReloadLabel
                        "Recharge CustomRoadNames.txt sans redémarrer le jeu. Les doublons sont ignorés. Le rechargement ne renomme pas les rues existantes.", // ReloadDesc
                        "Afficher le bouton des noms", // ShowButtonLabel
                        "Affiche un bouton pour une fenêtre déplaçable proposant des noms pour la rue sélectionnée. Cliquez sur une proposition pour l’appliquer.", // ShowButtonDesc
                        "Style sélectionné", // Disabled
                        "Noms personnalisés uniquement", // CustomOnly
                        "Mélanger la liste et le style", // Mixed
                        "Propositions de noms de rues", // Toggle
                        "Road Name Generator", // Title
                        "Nom actuel", // CurrentName
                        "Noms proposés", // Suggestions
                        "Appliquer", // Apply
                        "Appliquer ce nom à la rue sélectionnée.", // ApplyHint
                        "Nouvelles propositions", // Refresh
                        "Aucun nom inutilisé disponible. Ajoutez des noms à votre liste ou changez la source dans les options du mod.", // NoSuggestions
                        "Sélectionnez une rue avec l’outil d’information du jeu. Les propositions concernent toute la rue nommée et ses tronçons reliés.", // SelectRoad
                        "Noms personnalisés chargés", // CustomList
                        "CustomRoadNames.txt : un nom complet par ligne. Choisissez la source dans la liste déroulante ci-dessus.", // ListHint
                        "Ouvrir le dossier", // OpenFolder
                        "Recharger la liste", // ReloadList
                        "Impossible de lire la liste. Vérifiez le fichier, UTF-8 et les permissions. La dernière liste chargée avec succès est conservée.", // ListError
                        "Rue renommée.", // Applied
                        "La rue ou le nom n’est plus disponible. Les propositions ont été actualisées.", // Unavailable
                        "La sélection ou les paramètres ont changé. Les propositions ont été actualisées.", // Stale
                        "Aucune liste chargée", // NoList
                        "Style de noms", // NamingStyle
                        "Choisissez un style ou votre liste. Le choix est enregistré dans les options du mod et les propositions sont actualisées.", // NamingStyleHint
                        "Liste de noms personnalisés", // CustomListSource
                    };
                    break;
                case "it":
                    text = new[]
                    {
                        "Nomi stradali personalizzati", // CustomNamesGroup
                        "Singole strade", // SingleRoadGroup
                        "Origine dei nomi", // CustomNamesLabel
                        "Usa lo stile selezionato, solo il tuo elenco o una combinazione al 50 % finché sono disponibili nomi personalizzati. Con solo nomi personalizzati, le strade mantengono il nome attuale se l’elenco è vuoto o esaurito.", // CustomNamesDesc
                        "Apri cartella dei nomi", // OpenFolderLabel
                        "Apre ModsData/RoadNameGenerator. Modifica CustomRoadNames.txt in UTF-8: un nome completo per riga. Righe vuote, commenti # e nomi oltre 128 caratteri vengono ignorati.", // OpenFolderDesc
                        "Ricarica elenco personalizzato", // ReloadLabel
                        "Ricarica CustomRoadNames.txt senza riavviare il gioco. I duplicati vengono ignorati. Ricaricare non rinomina le strade esistenti.", // ReloadDesc
                        "Mostra pulsante dei nomi", // ShowButtonLabel
                        "Mostra un pulsante per una finestra spostabile con suggerimenti per la strada selezionata. Fai clic su un suggerimento per applicarlo.", // ShowButtonDesc
                        "Stile selezionato", // Disabled
                        "Solo nomi personalizzati", // CustomOnly
                        "Combina elenco e stile", // Mixed
                        "Suggerimenti per i nomi", // Toggle
                        "Road Name Generator", // Title
                        "Nome attuale", // CurrentName
                        "Nomi suggeriti", // Suggestions
                        "Applica", // Apply
                        "Applica questo nome alla strada selezionata.", // ApplyHint
                        "Nuovi suggerimenti", // Refresh
                        "Nessun nome inutilizzato disponibile. Aggiungi nomi all’elenco o cambia l’origine nelle opzioni del mod.", // NoSuggestions
                        "Seleziona una strada con lo strumento informazioni del gioco. I suggerimenti riguardano tutta la strada e i suoi tratti collegati.", // SelectRoad
                        "Nomi personalizzati caricati", // CustomList
                        "CustomRoadNames.txt: un nome completo per riga. Scegli l’origine nel menu a discesa in alto.", // ListHint
                        "Apri cartella", // OpenFolder
                        "Ricarica elenco", // ReloadList
                        "Impossibile leggere l’elenco. Controlla file, UTF-8 e permessi. Viene mantenuto l’ultimo elenco caricato correttamente.", // ListError
                        "Strada rinominata.", // Applied
                        "La strada o il nome non è più disponibile. I suggerimenti sono stati aggiornati.", // Unavailable
                        "La selezione o le impostazioni sono cambiate. I suggerimenti sono stati aggiornati.", // Stale
                        "Nessun elenco caricato", // NoList
                        "Stile dei nomi", // NamingStyle
                        "Scegli uno stile o il tuo elenco. La scelta viene salvata nelle opzioni del mod e aggiorna i suggerimenti.", // NamingStyleHint
                        "Elenco di nomi personalizzati", // CustomListSource
                    };
                    break;
                case "pl":
                    text = new[]
                    {
                        "Własne nazwy ulic", // CustomNamesGroup
                        "Pojedyncze ulice", // SingleRoadGroup
                        "Źródło nazw", // CustomNamesLabel
                        "Używa wybranego stylu, tylko własnej listy lub mieszanki 50/50, dopóki dostępne są własne nazwy. W trybie własnych nazw ulice zachowują obecną nazwę, gdy lista jest pusta lub wyczerpana.", // CustomNamesDesc
                        "Otwórz folder nazw", // OpenFolderLabel
                        "Otwiera ModsData/RoadNameGenerator. Edytuj CustomRoadNames.txt w UTF-8: jedna pełna nazwa na wiersz. Puste wiersze, komentarze # i nazwy dłuższe niż 128 znaków są pomijane.", // OpenFolderDesc
                        "Wczytaj ponownie własną listę", // ReloadLabel
                        "Wczytuje CustomRoadNames.txt bez ponownego uruchamiania gry. Duplikaty są pomijane. Ponowne wczytanie nie zmienia nazw istniejących ulic.", // ReloadDesc
                        "Pokaż przycisk nazw ulic", // ShowButtonLabel
                        "Pokazuje przycisk otwierający przesuwalne okno z propozycjami dla wybranej ulicy. Kliknij propozycję, aby ją zastosować.", // ShowButtonDesc
                        "Wybrany styl", // Disabled
                        "Tylko własne nazwy", // CustomOnly
                        "Połącz listę i styl", // Mixed
                        "Propozycje nazw ulic", // Toggle
                        "Road Name Generator", // Title
                        "Obecna nazwa", // CurrentName
                        "Proponowane nazwy", // Suggestions
                        "Zastosuj", // Apply
                        "Zastosuj tę nazwę do wybranej ulicy.", // ApplyHint
                        "Nowe propozycje", // Refresh
                        "Brak niewykorzystanych nazw. Dodaj nazwy do listy lub zmień źródło w opcjach moda.", // NoSuggestions
                        "Wybierz ulicę narzędziem informacji w grze. Propozycje dotyczą całej nazwanej ulicy wraz z połączonymi odcinkami.", // SelectRoad
                        "Wczytane własne nazwy", // CustomList
                        "CustomRoadNames.txt: jedna pełna nazwa na wiersz. Wybierz źródło z listy rozwijanej powyżej.", // ListHint
                        "Otwórz folder", // OpenFolder
                        "Wczytaj listę", // ReloadList
                        "Nie udało się odczytać listy. Sprawdź plik, UTF-8 i uprawnienia. Ostatnia poprawnie wczytana lista zostaje zachowana.", // ListError
                        "Zmieniono nazwę ulicy.", // Applied
                        "Ulica lub nazwa nie jest już dostępna. Propozycje zostały odświeżone.", // Unavailable
                        "Wybór lub ustawienia uległy zmianie. Propozycje zostały odświeżone.", // Stale
                        "Nie wczytano listy", // NoList
                        "Styl nazw", // NamingStyle
                        "Wybierz styl lub własną listę. Wybór jest zapisywany w opcjach moda, a propozycje są odświeżane.", // NamingStyleHint
                        "Własna lista nazw", // CustomListSource
                    };
                    break;
            }
            entries[setting.GetOptionGroupLocaleID(Setting.kCustomNamesGroup)] = text[0];
            entries[setting.GetOptionGroupLocaleID(Setting.kSingleRoadGroup)] = text[1];
            entries[setting.GetOptionLabelLocaleID(nameof(Setting.CustomNames))] = text[2];
            entries[setting.GetOptionDescLocaleID(nameof(Setting.CustomNames))] = text[3];
            entries[setting.GetOptionLabelLocaleID(nameof(Setting.OpenCustomNamesFolder))] = text[4];
            entries[setting.GetOptionDescLocaleID(nameof(Setting.OpenCustomNamesFolder))] = text[5];
            entries[setting.GetOptionLabelLocaleID(nameof(Setting.ReloadCustomNames))] = text[6];
            entries[setting.GetOptionDescLocaleID(nameof(Setting.ReloadCustomNames))] = text[7];
            entries[setting.GetOptionLabelLocaleID(nameof(Setting.ShowRoadSuggestionsButton))] = text[8];
            entries[setting.GetOptionDescLocaleID(nameof(Setting.ShowRoadSuggestionsButton))] = text[9];
            entries[setting.GetEnumValueLocaleID(CustomNameMode.Disabled)] = text[10];
            entries[setting.GetEnumValueLocaleID(CustomNameMode.CustomOnly)] = text[11];
            entries[setting.GetEnumValueLocaleID(CustomNameMode.Mixed)] = text[12];
            string[] keys =
            {
                "RoadNameGenerator.UI.Toggle",
                "RoadNameGenerator.UI.Title",
                "RoadNameGenerator.UI.CurrentName",
                "RoadNameGenerator.UI.Suggestions",
                "RoadNameGenerator.UI.Apply",
                "RoadNameGenerator.UI.ApplyHint",
                "RoadNameGenerator.UI.Refresh",
                "RoadNameGenerator.UI.NoSuggestions",
                "RoadNameGenerator.UI.SelectRoad",
                "RoadNameGenerator.UI.CustomList",
                "RoadNameGenerator.UI.ListHint",
                "RoadNameGenerator.UI.OpenFolder",
                "RoadNameGenerator.UI.ReloadList",
                "RoadNameGenerator.UI.ListError",
                "RoadNameGenerator.UI.Applied",
                "RoadNameGenerator.UI.Unavailable",
                "RoadNameGenerator.UI.Stale",
                "RoadNameGenerator.UI.NoList",
                "RoadNameGenerator.UI.NamingStyle",
                "RoadNameGenerator.UI.NamingStyleHint",
                "RoadNameGenerator.UI.CustomSource",
            };
            for (int i = 0; i < keys.Length; i++)
                entries[keys[i]] = text[i + 13];
            entries["RoadNameGenerator.UI.MixedSource"] = text[12];
            string[] exclusions = language switch
            {
                "de" => new[]
                {
                    "Geladene Ausschlussregeln",
                    "Keine Ausschlussliste aktiv",
                    "ExcludedRoadNames.txt: ein Name oder Muster pro Zeile. * steht für beliebigen Text, z. B. Autoroute *. Groß-/Kleinschreibung wird ignoriert.",
                    "Ausschlüsse laden",
                    "Entfernen",
                    "Deaktiviert die Ausschlussliste. Die Datei bleibt erhalten und kann erneut geladen werden.",
                    "Kein erlaubter ungenutzter Name gefunden. Ausschlussregeln prüfen, eigene Namen ergänzen oder eine andere Namensquelle wählen.",
                    "Ausschlussliste verwenden",
                    "Wendet ExcludedRoadNames.txt auf alle Namensstile und eigene Namen an. Bestehende Straßen ändern sich erst beim bewussten erneuten Benennen.",
                    "Ausschlussliste neu laden",
                    "Lädt und aktiviert ExcludedRoadNames.txt aus dem Namenslistenordner (UTF-8). Ein vollständiger Name oder *-Muster pro Zeile; Leerzeilen und #-Kommentare werden ignoriert. Das Neuladen benennt keine bestehenden Straßen um.",
                },
                "es" => new[]
                {
                    "Reglas de exclusión cargadas",
                    "Ninguna lista de exclusión activa",
                    "ExcludedRoadNames.txt: un nombre o patrón por línea. * representa cualquier texto, p. ej. Autoroute *. No distingue mayúsculas de minúsculas.",
                    "Cargar exclusiones",
                    "Quitar",
                    "Desactiva las exclusiones. El archivo se conserva y puede cargarse de nuevo.",
                    "No se encontró ningún nombre permitido sin usar. Revisa las exclusiones, añade nombres o cambia el origen.",
                    "Usar lista de exclusión",
                    "Aplica ExcludedRoadNames.txt a todos los estilos y nombres personalizados. Las calles existentes solo cambian al renombrarlas expresamente.",
                    "Recargar exclusiones",
                    "Carga y activa ExcludedRoadNames.txt desde la carpeta de nombres (UTF-8). Un nombre completo o patrón con * por línea; ignora líneas vacías y comentarios #. No renombra calles existentes.",
                },
                "fr" => new[]
                {
                    "Règles d’exclusion chargées",
                    "Aucune liste d’exclusion active",
                    "ExcludedRoadNames.txt : un nom ou motif par ligne. * représente tout texte, p. ex. Autoroute *. La casse est ignorée.",
                    "Charger exclusions",
                    "Retirer",
                    "Désactive les exclusions. Le fichier est conservé et peut être rechargé.",
                    "Aucun nom autorisé inutilisé trouvé. Vérifiez les exclusions, ajoutez des noms ou changez la source.",
                    "Utiliser la liste d’exclusion",
                    "Applique ExcludedRoadNames.txt à tous les styles et noms personnalisés. Les rues existantes ne changent que lors d’un renommage explicite.",
                    "Recharger les exclusions",
                    "Charge et active ExcludedRoadNames.txt depuis le dossier des noms (UTF-8). Un nom complet ou motif avec * par ligne ; ignore les lignes vides et commentaires #. Ne renomme pas les rues existantes.",
                },
                "it" => new[]
                {
                    "Regole di esclusione caricate",
                    "Nessun elenco di esclusione attivo",
                    "ExcludedRoadNames.txt: un nome o modello per riga. * indica qualsiasi testo, ad es. Autoroute *. Ignora maiuscole e minuscole.",
                    "Carica esclusioni",
                    "Rimuovi",
                    "Disattiva le esclusioni. Il file viene conservato e può essere ricaricato.",
                    "Nessun nome consentito inutilizzato trovato. Controlla le esclusioni, aggiungi nomi o cambia origine.",
                    "Usa elenco di esclusione",
                    "Applica ExcludedRoadNames.txt a tutti gli stili e nomi personalizzati. Le strade esistenti cambiano solo quando rinominate esplicitamente.",
                    "Ricarica esclusioni",
                    "Carica e attiva ExcludedRoadNames.txt dalla cartella dei nomi (UTF-8). Un nome completo o modello con * per riga; ignora righe vuote e commenti #. Non rinomina strade esistenti.",
                },
                "pl" => new[]
                {
                    "Wczytane reguły wykluczeń",
                    "Brak aktywnej listy wykluczeń",
                    "ExcludedRoadNames.txt: jedna nazwa lub wzorzec w wierszu. * oznacza dowolny tekst, np. Autoroute *. Wielkość liter jest ignorowana.",
                    "Wczytaj wykluczenia",
                    "Usuń",
                    "Wyłącza wykluczenia. Plik zostaje zachowany i można go wczytać ponownie.",
                    "Nie znaleziono dozwolonej niewykorzystanej nazwy. Sprawdź wykluczenia, dodaj nazwy lub zmień źródło.",
                    "Używaj listy wykluczeń",
                    "Stosuje ExcludedRoadNames.txt do wszystkich stylów i własnych nazw. Istniejące ulice zmieniają się tylko po jawnym ponownym nazwaniu.",
                    "Wczytaj ponownie wykluczenia",
                    "Wczytuje i włącza ExcludedRoadNames.txt z folderu nazw (UTF-8). Jedna pełna nazwa lub wzorzec z * na wiersz; pomija puste wiersze i komentarze #. Nie zmienia nazw istniejących ulic.",
                },
                _ => new[]
                {
                    "Loaded exclusion rules",
                    "No exclusion list active",
                    "ExcludedRoadNames.txt: one name or pattern per line. * matches any text, e.g. Autoroute *. Matching ignores letter case.",
                    "Load exclusions",
                    "Remove",
                    "Disables the exclusion list. The file is kept and can be loaded again.",
                    "No allowed unused name found. Check the exclusion rules, add custom names or choose another name source.",
                    "Use name exclusion list",
                    "Applies ExcludedRoadNames.txt to all naming styles and custom names. Existing roads change only when explicitly renamed.",
                    "Reload exclusion list",
                    "Loads and enables ExcludedRoadNames.txt from the name list folder (UTF-8). One complete name or * pattern per line; empty lines and # comments are ignored. Reloading does not rename existing roads.",
                },
            };
            entries["RoadNameGenerator.UI.ExclusionList"] = exclusions[0];
            entries["RoadNameGenerator.UI.NoExclusions"] = exclusions[1];
            entries["RoadNameGenerator.UI.ExclusionHint"] = exclusions[2];
            entries["RoadNameGenerator.UI.ReloadExclusions"] = exclusions[3];
            entries["RoadNameGenerator.UI.RemoveExclusions"] = exclusions[4];
            entries["RoadNameGenerator.UI.RemoveExclusionsHint"] = exclusions[5];
            entries["RoadNameGenerator.UI.ExclusionsUnavailable"] = exclusions[6];
            entries[setting.GetOptionLabelLocaleID(nameof(Setting.UseNameExclusions))] = exclusions[7];
            entries[setting.GetOptionDescLocaleID(nameof(Setting.UseNameExclusions))] = exclusions[8];
            entries[setting.GetOptionLabelLocaleID(nameof(Setting.ReloadExcludedNames))] = exclusions[9];
            entries[setting.GetOptionDescLocaleID(nameof(Setting.ReloadExcludedNames))] = exclusions[10];

            string[] protection = language switch
            {
                "de" => new[] { "Einzeln benannte Straßen überschreiben", "Erlaubt der Bestandsprüfung, einzeln eingegebene oder über Namensvorschläge gewählte Namen zu ersetzen. Standardmäßig bleiben sie geschützt, auch bei „Alle Straßen“. Automatisch durch diese Version vergebene Namen können weiterhin geändert werden. Namen aus älteren Versionen ohne Herkunftskennzeichnung gelten als geschützt. Der gewählte Prüfmodus gilt weiterhin; „Nur vom Spiel generierte Namen“ überspringt alle benutzerdefinierten Namen." },
                "fr" => new[] { "Remplacer les noms attribués individuellement", "Autorise la vérification à remplacer les noms saisis manuellement ou choisis parmi les suggestions. Par défaut, ils restent protégés, même en mode Toutes les voies. Les noms attribués automatiquement par cette version restent modifiables. Les anciens noms personnalisés sans origine connue sont protégés. Le mode sélectionné reste applicable ; le mode Noms du jeu uniquement ignore tous les noms personnalisés." },
                "it" => new[] { "Sovrascrivi i nomi assegnati singolarmente", "Consente alla verifica di sostituire i nomi inseriti manualmente o scelti dai suggerimenti. Per impostazione predefinita restano protetti anche in modalità Tutte le strade. I nomi assegnati automaticamente da questa versione restano modificabili. I vecchi nomi personalizzati di origine sconosciuta sono protetti. La modalità selezionata resta valida; Solo nomi del gioco ignora tutti i nomi personalizzati." },
                "es" => new[] { "Sobrescribir nombres asignados individualmente", "Permite que la revisión sustituya nombres escritos manualmente o elegidos entre las sugerencias. Por defecto quedan protegidos incluso en el modo Todas las vías. Los nombres asignados automáticamente por esta versión siguen siendo modificables. Los nombres personalizados antiguos sin origen conocido están protegidos. El modo seleccionado sigue aplicándose; Solo nombres del juego omite todos los nombres personalizados." },
                "pl" => new[] { "Nadpisuj nazwy nadane pojedynczym drogom", "Pozwala kontroli zastępować nazwy wpisane ręcznie lub wybrane z propozycji. Domyślnie pozostają chronione także w trybie Wszystkie drogi. Nazwy nadane automatycznie przez tę wersję nadal można zmieniać. Starsze własne nazwy o nieznanym pochodzeniu są chronione. Wybrany tryb nadal obowiązuje; Tylko nazwy z gry pomija wszystkie własne nazwy." },
                _ => new[] { "Overwrite individually named roads", "Allows the existing-road scan to replace names entered manually or selected from suggestions. By default they remain protected, even in All roads mode. Names automatically assigned by this version can still be changed. Older custom names without provenance are protected. The selected scan mode still applies; Game-generated only skips all custom names." },
            };
            entries[setting.GetOptionLabelLocaleID(nameof(Setting.OverwriteIndividuallyNamedRoads))] = protection[0];
            entries[setting.GetOptionDescLocaleID(nameof(Setting.OverwriteIndividuallyNamedRoads))] = protection[1];
            // Compact in-game panel and its separate options window.
            string[] compactKeys = { "SettingsTitle", "RenameAll", "CheckExisting", "ConfirmScan", "Confirm", "Cancel", "AutomaticNamingSection", "AutomaticNaming", "ExistingRoadsSection", "ScanMode", "ScanGameOnly", "ScanNonMatching", "ScanAll", "OverwriteNames", "ScanExplanation", "CustomListsSection", "DetailedLogging", "ScanQueued" };
            string[] compactText = language switch
            {
                "de" => new[] { "Einstellungen", "Alle umbenennen", "Bestehende prüfen", "Diesen Scan wirklich starten? Modus:", "Jetzt ausführen", "Abbrechen", "Automatische Benennung", "Neue Straßen automatisch benennen", "Bestehende Straßen", "Prüfmodus", "Nur Spielnamen", "Nicht passende Namen", "Alle Straßen", "Individuelle Namen überschreiben", "Manuell vergebene Namen bleiben geschützt, sofern das Überschreiben ausgeschaltet ist.", "Namenslisten", "Detailliertes Logging", "Prüfung bestehender Straßen angefordert." },
                "fr" => new[] { "Paramètres", "Renommer toutes les routes", "Vérifier les routes", "Lancer l’analyse en mode :", "Lancer", "Annuler", "Nommage automatique", "Nommer automatiquement les nouvelles routes", "Routes existantes", "Mode d’analyse", "Noms générés par le jeu", "Noms hors du style choisi", "Toutes les routes", "Écraser les noms personnalisés", "Les noms attribués manuellement sont protégés sauf si leur remplacement est activé.", "Listes de noms", "Journal détaillé", "Analyse des routes demandée." },
                "es" => new[] { "Ajustes", "Renombrar todas las vías", "Revisar vías existentes", "¿Iniciar la revisión en modo:", "Iniciar", "Cancelar", "Nombres automáticos", "Nombrar automáticamente vías nuevas", "Vías existentes", "Modo de revisión", "Solo nombres del juego", "Nombres fuera del estilo", "Todas las vías", "Sobrescribir nombres individuales", "Los nombres asignados manualmente están protegidos salvo que se permita sobrescribirlos.", "Listas de nombres", "Registro detallado", "Revisión de vías solicitada." },
                "it" => new[] { "Impostazioni", "Rinomina tutte le strade", "Controlla strade esistenti", "Avviare il controllo in modalità:", "Avvia", "Annulla", "Denominazione automatica", "Assegna nomi alle nuove strade", "Strade esistenti", "Modalità di controllo", "Solo nomi del gioco", "Nomi fuori dallo stile", "Tutte le strade", "Sovrascrivi nomi personalizzati", "I nomi assegnati manualmente sono protetti se la sovrascrittura è disattivata.", "Elenchi nomi", "Registro dettagliato", "Controllo delle strade richiesto." },
                "pl" => new[] { "Ustawienia", "Zmień nazwy wszystkich dróg", "Sprawdź istniejące drogi", "Uruchomić skanowanie w trybie:", "Uruchom", "Anuluj", "Automatyczne nazwy", "Automatycznie nazywaj nowe drogi", "Istniejące drogi", "Tryb skanowania", "Tylko nazwy z gry", "Nazwy niezgodne ze stylem", "Wszystkie drogi", "Nadpisuj nazwy własne", "Nazwy nadane ręcznie są chronione, chyba że włączono nadpisywanie.", "Listy nazw", "Szczegółowe logowanie", "Zlecono sprawdzenie istniejących dróg." },
                _ => new[] { "Settings", "Rename all roads", "Check existing roads", "Start road scan with mode:", "Start scan", "Cancel", "Automatic naming", "Automatically name new roads", "Existing roads", "Scan mode", "Game-generated names only", "Names outside selected style", "All roads", "Overwrite individually named roads", "Player-assigned names are protected unless overwriting is enabled.", "Name lists", "Detailed logging", "Existing-road scan requested." }
            };
            for (int i = 0; i < compactKeys.Length; i++)
                entries["RoadNameGenerator.UI." + compactKeys[i]] = compactText[i];

            entries["RoadNameGenerator.UI.ConfirmAgain"] = language switch
            {
                "de" => "Zum Bestätigen erneut klicken",
                "fr" => "Cliquer à nouveau pour confirmer",
                "es" => "Pulsa de nuevo para confirmar",
                "it" => "Clicca di nuovo per confermare",
                "pl" => "Kliknij ponownie, aby potwierdzić",
                _ => "Click again to confirm"
            };

            entries["RoadNameGenerator.UI.UseExclusions"] = language switch
            {
                "de" => "Ausschlussliste verwenden",
                "fr" => "Utiliser la liste d’exclusion",
                "es" => "Usar lista de exclusiones",
                "it" => "Usa elenco di esclusione",
                "pl" => "Używaj listy wykluczeń",
                _ => "Use exclusion list"
            };

            return entries;
        }
    }
}

