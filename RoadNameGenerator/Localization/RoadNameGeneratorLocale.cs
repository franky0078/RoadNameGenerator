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
            return entries;
        }
    }
}
