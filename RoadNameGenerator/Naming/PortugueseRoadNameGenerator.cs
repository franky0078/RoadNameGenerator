// Road Name Generator
// Copyright (C) 2026 franky0078
// Licensed under the GNU General Public License version 3 or later.

namespace RoadNameGenerator.Naming
{
    // European Portuguese: endings contain their complete prepositions and
    // articles so road types never produce incorrect combinations such as
    // "de a". These are generated names inspired by Portugal, not an address map.
    public sealed class PortugueseRoadNameGenerator : RegionalRoadNameGenerator
    {
        private static readonly string[] s_Nature =
        {
            "do Pinhal", "dos Pinheiros", "dos Carvalhos",
            "dos Sobreiros", "das Azinheiras", "dos Castanheiros",
            "das Oliveiras", "dos Freixos", "dos Salgueiros",
            "dos Plátanos", "dos Cedros", "dos Ciprestes",
            "dos Loureiros", "das Amendoeiras", "das Figueiras",
            "das Laranjeiras", "dos Limoeiros", "das Nogueiras",
            "das Cerejeiras", "das Pereiras", "das Macieiras",
            "dos Medronheiros", "das Acácias", "das Tílias",
            "das Camélias", "das Hortênsias", "das Magnólias",
            "das Rosas", "dos Cravos", "dos Lírios",
            "das Violetas", "das Papoilas", "das Margaridas",
            "das Amoras", "das Giestas", "do Rosmaninho",
            "do Alecrim", "da Alfazema", "dos Jasmins",
            "das Flores", "da Várzea", "da Ribeira",
            "do Ribeiro", "da Fonte", "das Fontes",
            "da Nascente", "do Poço", "do Vale",
            "da Serra", "do Monte", "da Encosta",
            "do Outeiro", "da Colina", "do Bosque",
            "da Mata", "do Souto", "da Lezíria",
            "dos Campos", "da Quinta", "das Quintas",
            "da Horta", "das Hortas", "do Pomar",
            "da Vinha", "das Vinhas", "do Olival",
            "dos Moinhos", "da Azenha", "da Eira",
            "do Celeiro", "da Lavoura", "do Prado",
            "da Lagoa", "do Lago", "do Rio",
            "da Margem", "do Mar", "da Praia",
            "das Dunas", "da Falésia", "dos Pescadores",
            "das Gaivotas", "das Andorinhas", "dos Rouxinóis",
            "do Miradouro", "da Boavista", "do Sol",
            "da Lua", "dos Azevinhos", "dos Teixos",
            "das Bétulas", "dos Choupos", "dos Ulmeiros",
            "das Faias", "dos Eucaliptos", "das Alfarrobeiras",
            "das Romãzeiras", "das Nespereiras", "dos Marmeleiros",
            "dos Pessegueiros", "dos Damasqueiros", "das Ameixeiras",
            "das Tangerineiras", "dos Fetos", "dos Zimbros",
            "dos Tojos", "das Urzes", "das Estevas",
            "das Camarinhas", "das Malvas", "das Dálias",
            "das Tulipas", "dos Narcisos", "das Begónias",
            "das Petúnias", "dos Gerânios", "das Açucenas",
            "dos Amores-Perfeitos", "das Orquídeas", "das Frésias",
            "das Madressilvas", "das Buganvílias", "do Tomilho",
            "da Hortelã", "da Erva-Doce", "dos Girassóis",
            "do Trevo", "dos Canaviais", "dos Juncos",
            "da Charneca", "do Montado", "da Veiga",
            "do Lameiro", "das Pastagens", "da Seara",
            "dos Trigais", "dos Milheirais", "do Arrozal",
            "do Lagar", "dos Lagares", "do Fontanário",
            "da Cascata", "da Foz", "do Estuário",
            "da Enseada", "da Baía", "do Cabo",
            "da Arriba", "do Farol", "do Cais",
            "da Maré", "da Brisa", "do Horizonte",
            "das Estrelas", "do Luar", "do Poente",
            "do Nascente", "do Alvorecer", "das Cotovias",
            "dos Melros", "dos Pintassilgos", "dos Chapins",
            "das Cegonhas", "das Garças", "dos Flamingos",
            "das Águias", "dos Falcões", "dos Mochos",
            "das Corujas", "das Perdizes", "das Rolas",
            "dos Tordos", "dos Pombos", "das Lebres",
            "dos Coelhos", "dos Veados", "das Lontras",
            "dos Esquilos", "das Raposas", "do Barranco",
            "da Cumeada", "da Portela", "da Ladeira",
            "da Pedra", "do Granito", "do Xisto",
            "do Basalto", "das Salinas", "das Marinhas",
            "dos Sapais", "da Penha", "da Fraga",
            "do Cabeço", "da Lapa"
        };

        private static readonly string[] s_Places =
        {
            "da Igreja", "da Capela", "do Mosteiro",
            "do Convento", "do Castelo", "da Torre",
            "da Muralha", "da Ponte", "do Aqueduto",
            "da Estação", "do Mercado", "da Feira",
            "da Escola", "da Biblioteca", "do Teatro",
            "do Museu", "do Jardim", "do Parque",
            "do Rossio", "do Terreiro", "da Sé",
            "do Carmo", "da Misericórdia", "da Esperança",
            "da Liberdade", "da República", "da Independência",
            "da Restauração", "da Constituição", "da União",
            "da Paz", "da Saudade", "da Alegria",
            "da Amizade", "da Primavera", "da Aurora",
            "do Comércio", "dos Sapateiros", "dos Correeiros",
            "dos Douradores", "dos Oleiros", "dos Ferreiros",
            "dos Carpinteiros", "dos Tecelões", "dos Lavradores",
            "dos Navegantes", "das Descobertas", "dos Descobrimentos",
            "de São João", "de São Pedro", "de São José",
            "de São Miguel", "de São Vicente", "de São Sebastião",
            "de Santo António", "de São Bento", "de Santa Catarina",
            "de Santa Clara", "de Santa Luzia", "de Santa Maria",
            "da Conceição", "das Águas Livres", "de Portugal",
            "de Lisboa", "do Porto", "de Coimbra",
            "de Braga", "de Guimarães", "de Aveiro",
            "de Viseu", "de Évora", "de Beja",
            "de Faro", "de Setúbal", "de Santarém",
            "de Leiria", "de Castelo Branco", "de Portalegre",
            "de Bragança", "de Vila Real", "de Viana do Castelo",
            "da Guarda", "de Sintra", "de Cascais",
            "de Tomar", "de Óbidos", "de Lagos",
            "de Tavira", "do Minho", "do Douro",
            "do Mondego", "do Tejo", "do Sado",
            "do Guadiana", "do Alentejo", "do Algarve",
            "da Beira", "da Estremadura", "de Trás-os-Montes",
            "da Madeira", "dos Açores", "do Funchal",
            "de Ponta Delgada", "de Angra do Heroísmo", "do Pelourinho",
            "do Chafariz", "do Coreto", "da Ermida",
            "da Abadia", "do Cruzeiro", "da Sacristia",
            "do Adro", "do Campanário", "da Colegiada",
            "da Reitoria", "da Universidade", "do Liceu",
            "do Conservatório", "da Academia", "do Observatório",
            "do Planetário", "do Hospital", "do Posto Médico",
            "dos Bombeiros", "da Alfândega", "dos Correios",
            "do Tribunal", "da Câmara", "da Junta",
            "da Cooperativa", "do Matadouro", "da Adega",
            "da Taberna", "da Padaria", "da Fábrica",
            "da Oficina", "do Armazém", "do Estaleiro",
            "da Doca", "do Porto de Pesca", "da Marina",
            "do Passeio", "da Calçada", "do Largo",
            "da Praça", "da Rotunda", "da Arcada",
            "das Escadinhas", "do Pátio", "da Vila",
            "da Aldeia", "do Bairro", "do Bairro Novo",
            "da Cerca", "da Tapada", "da Coutada",
            "do Casal", "da Vala", "do Regato",
            "da Levada", "da Quelha", "do Boqueirão",
            "da Boavista", "do Bom Sucesso", "da Vitória",
            "da Glória", "da Harmonia", "da Fraternidade",
            "da Solidariedade", "da Igualdade", "da Democracia",
            "da Instrução", "do Progresso", "do Trabalho",
            "dos Artistas", "dos Poetas", "dos Escritores",
            "dos Músicos", "dos Pintores", "dos Escultores",
            "dos Professores", "dos Estudantes", "dos Mestres",
            "dos Aprendizes", "dos Ourives", "dos Caldeireiros",
            "dos Tanoeiros", "dos Cesteiros", "dos Serralheiros",
            "dos Alfaiates", "das Costureiras", "das Lavadeiras",
            "das Varinas", "dos Almocreves", "dos Moleiros",
            "dos Hortelãos", "dos Pastores", "dos Marinheiros",
            "dos Mercadores", "da Prata", "do Ouro",
            "de Chaves", "de Lamego", "de Amarante",
            "de Barcelos", "de Valença", "de Ponte de Lima",
            "de Ponte da Barca", "de Vila do Conde", "de Póvoa de Varzim",
            "de Espinho", "de Matosinhos", "de Penafiel",
            "de Felgueiras", "de Santo Tirso", "de Fafe",
            "de Famalicão", "de Vila Nova de Gaia", "de Gondomar",
            "de Ovar", "de Águeda", "de Oliveira de Azeméis",
            "de São João da Madeira", "da Covilhã", "do Fundão",
            "de Belmonte", "de Seia", "de Gouveia",
            "de Abrantes", "de Torres Novas", "de Torres Vedras",
            "de Peniche", "da Nazaré", "de Alcobaça",
            "da Batalha", "de Mafra", "de Loures",
            "de Almada", "do Barreiro", "de Sesimbra",
            "de Palmela", "de Sines", "de Santiago do Cacém",
            "de Estremoz", "de Elvas", "de Moura",
            "de Serpa", "de Mértola", "de Silves",
            "de Portimão", "de Albufeira", "de Loulé",
            "de Olhão", "de Vila Real de Santo António", "de Santana",
            "de Machico", "da Ribeira Brava",
            "de Câmara de Lobos", "da Calheta", "da Ribeira Grande",
            "da Ilha do Faial", "de Vila Franca do Campo", "de Santa Cruz da Graciosa",
            "das Lajes do Pico", "das Velas", "de Vila do Porto",
            "do Lima", "do Cávado", "do Ave",
            "do Tâmega", "do Vouga", "do Zêzere",
            "do Nabão", "do Sorraia", "do Mira",
            "do Arade", "da Ria de Aveiro", "da Ria Formosa",
            "da Serra da Estrela", "da Serra do Gerês", "da Serra de Sintra",
            "da Serra da Arrábida", "do Ribatejo", "de Entre Douro e Minho",
            "de São Tiago", "de São Martinho", "de São Lourenço",
            "de São Francisco", "de Santa Bárbara", "de Santa Teresa",
            "de Nossa Senhora de Fátima", "de Nossa Senhora da Nazaré", "de Nossa Senhora do Rosário",
            "do Espírito Santo", "de 25 de Abril", "de 1.º de Maio",
            "de 5 de Outubro", "de 1.º de Dezembro", "de 10 de Junho"
        };

        private static readonly string[] s_People =
        {
            "de Luís de Camões", "de Fernando Pessoa", "de Eça de Queirós",
            "de José Saramago", "de Almeida Garrett", "de Camilo Castelo Branco",
            "de Alexandre Herculano", "de Antero de Quental", "de Cesário Verde",
            "de Bocage", "de Florbela Espanca", "de Sophia de Mello Breyner Andresen",
            "de Miguel Torga", "de Eugénio de Andrade", "de António Nobre",
            "de José Régio", "de Aquilino Ribeiro", "de Raul Brandão",
            "de Vergílio Ferreira", "de Agustina Bessa-Luís", "de Natália Correia",
            "de Mário de Sá-Carneiro", "de Teixeira de Pascoaes", "de Gil Vicente",
            "de Fernão Mendes Pinto", "de António Sérgio", "de Amália Rodrigues",
            "de Carlos Paredes", "de José Afonso", "de Adriano Correia de Oliveira",
            "de Alfredo Marceneiro", "de Maria Severa", "de Maria João Pires",
            "de José Vianna da Motta", "de Luís de Freitas Branco", "de Júlio Dinis",
            "de Almada Negreiros", "de Amadeo de Souza-Cardoso", "de Columbano Bordalo Pinheiro",
            "de Rafael Bordalo Pinheiro", "de Paula Rego", "de Vieira da Silva",
            "de Nadir Afonso", "de José Malhoa", "de Silva Porto",
            "de Dom Afonso Henriques", "de Dom Dinis", "de Dom João I",
            "de Dom João II", "de Dom Manuel I", "de Dom Pedro IV",
            "da Rainha Santa Isabel", "do Infante Dom Henrique", "de Vasco da Gama",
            "de Pedro Álvares Cabral", "de Fernão de Magalhães", "de Bartolomeu Dias",
            "de Diogo Cão", "de Gil Eanes", "de Damião de Góis",
            "de Afonso de Albuquerque", "de Nuno Álvares Pereira", "de Viriato",
            "do Marquês de Pombal", "de Mouzinho da Silveira", "de Fontes Pereira de Melo",
            "de Passos Manuel", "de Manuel Fernandes Tomás", "de António José de Almeida",
            "de Afonso Costa", "de Bernardino Machado", "de João de Deus",
            "de Maria da Fonte", "de Carolina Beatriz Ângelo", "de Adelaide Cabete",
            "de Ana de Castro Osório", "de Egas Moniz", "de Garcia de Orta",
            "de Pedro Nunes", "de Gago Coutinho", "de Sacadura Cabral",
            "de Abel Salazar", "de António Gedeão", "de Rómulo de Carvalho",
            "de Bento de Jesus Caraça", "de Salgueiro Maia", "de Aristides de Sousa Mendes",
            "de Humberto Delgado", "de Mário Soares", "de Eusébio",
            "de Rosa Mota", "de Carlos Lopes", "de Joaquim Agostinho",
            "de António Aleixo", "de Manuel da Fonseca", "de Alves Redol",
            "de Ferreira de Castro", "de Fernando Namora", "de Carlos de Oliveira",
            "de José Gomes Ferreira", "de Jorge de Sena", "de Vitorino Nemésio",
            "de Daniel Faria", "de Eugénio de Castro", "de António Botto",
            "de Irene Lisboa", "de Maria Lamas", "de Maria Judite de Carvalho",
            "de Maria Velho da Costa", "de Maria Teresa Horta", "de Maria Isabel Barreno",
            "de Fiama Hasse Pais Brandão", "de Ana Hatherly", "de António Ramos Rosa",
            "de Herberto Helder", "de Manuel Alegre", "de Alexandre O'Neill",
            "de David Mourão-Ferreira", "de José Cardoso Pires", "de Urbano Tavares Rodrigues",
            "de Joaquim Manuel Magalhães", "de Gonçalo M. Tavares", "de Lídia Jorge",
            "de Dulce Maria Cardoso", "de Hélia Correia", "de Teolinda Gersão",
            "de Alice Vieira", "de António Lobo Antunes", "de José Luís Peixoto",
            "de Fernando Lopes-Graça", "de Joly Braga Santos", "de Francisco de Lacerda",
            "de José António Carlos de Seixas", "de António Fragoso", "de José Mário Branco",
            "de Sérgio Godinho", "de Fausto Bordalo Dias", "de Carlos do Carmo",
            "de Hermínia Silva", "de Lucília do Carmo", "de Beatriz Costa",
            "de António Silva", "de Vasco Santana", "de Ribeirinho",
            "de Laura Alves", "de Eunice Muñoz", "de Ruy de Carvalho",
            "de Raul Solnado", "de Manoel de Oliveira", "de João César Monteiro",
            "de Fernando Lopes", "de António Reis", "de Margarida Cordeiro",
            "de José Leitão de Barros", "de Artur Ramos", "de João Cutileiro",
            "de Soares dos Reis", "de Leopoldo de Almeida", "de José de Guimarães",
            "de Júlio Pomar", "de Mário Cesariny", "de António Dacosta",
            "de Domingos Sequeira", "de Henrique Pousão", "de Aurélia de Sousa",
            "de António Carneiro", "de Carlos Botelho", "de Eduardo Viana",
            "de Dordio Gomes", "de Sarah Afonso", "de Jorge Barradas",
            "de Cottinelli Telmo", "de Pardal Monteiro", "de Cassiano Branco",
            "de Raul Lino", "de Álvaro Siza Vieira", "de Eduardo Souto de Moura",
            "de Gonçalo Ribeiro Telles", "de Francisco Keil do Amaral", "de José Marques da Silva",
            "de Ventura Terra", "de José Luís Monteiro", "de Carlos Mardel",
            "de Eugénio dos Santos", "de António Augusto de Aguiar", "de Serpa Pinto",
            "de Roberto Ivens", "de Hermenegildo Capelo", "de Brito Capelo",
            "de João de Barros", "de Fernão Lopes", "de Gomes Eanes de Zurara",
            "de João de Castilho", "de Diogo de Boitaca", "de Diogo de Arruda",
            "de Diogo de Torralva", "de Duarte Pacheco Pereira", "de Tristão da Cunha",
            "de Gonçalo Velho Cabral", "de João Gonçalves Zarco", "de Tristão Vaz Teixeira",
            "de Gonçalo Anes Bandarra", "de Francisco de Almeida", "de Francisco de Sá de Miranda",
            "de António Ferreira", "de Dom Duarte", "de Dom João IV",
            "de Dom Pedro V", "de Dom Luís I", "da Rainha Dona Leonor",
            "da Rainha Dona Maria II", "de Afonso de Paiva", "de Pêro da Covilhã",
            "de Dinis Dias", "de João de Santarém", "de Pêro Escobar",
            "de Henrique Galvão", "de Francisco Sá Carneiro", "de Adelino Amaro da Costa",
            "de António de Spínola", "de Francisco da Costa Gomes",
            "de Vítor Alves", "de Ernesto Melo Antunes", "de José Relvas",
            "de José Falcão", "de António Granjo", "de António Maria da Silva",
            "de Manuel Teixeira Gomes", "de António de Oliveira Marreca", "de José Estevão",
            "de Hintze Ribeiro", "de Oliveira Martins", "de António Feliciano de Castilho",
            "de Ramalho Ortigão", "de Guerra Junqueiro", "de Júlio de Castilho"
        };

        // Combinations removed after the European Portuguese language review.
        private static readonly System.Collections.Generic.HashSet<string> s_ExcludedNames =
            new(System.StringComparer.OrdinalIgnoreCase)
        {
            "Praceta da Praça",
            "Praceta da Quelha",
            "Praceta das Escadinhas",
            "Calçada da Calçada",
            "Calçada da Quelha",
            "Calçada das Escadinhas",
            "Estrada da Calçada",
            "Estrada da Quelha",
            "Estrada das Escadinhas",
            "Estrada do Largo",
            "Estrada do Pátio",
            "Alameda da Calçada",
            "Alameda da Quelha",
            "Alameda das Escadinhas",
            "Alameda do Largo",
            "Alameda do Pátio",
            "Avenida da Calçada",
            "Avenida da Quelha",
            "Avenida das Escadinhas",
            "Avenida do Largo",
            "Avenida do Pátio"
        };

        private static string[][] CreateNamePools()
        {
            string[][] pools =
            new[]
            {
                Prefix(s_Nature, "Caminho ", "Estrada ", "Vereda "),
                Join(Prefix(s_Nature, "Travessa ", "Beco "),
                     Prefix(s_Places, "Travessa ", "Beco ")),
                Join(Prefix(s_Nature, "Rua ", "Praceta "),
                     Prefix(s_Places, "Rua ", "Praceta "),
                     Prefix(s_People, "Rua ")),
                Join(Prefix(s_Nature, "Rua ", "Estrada "),
                     Prefix(s_Places, "Rua ", "Estrada ", "Calçada "),
                     Prefix(s_People, "Rua ", "Calçada "),
                     PortugueseRouteNames.NationalRoads),
                Join(Prefix(s_Places, "Avenida ", "Alameda "),
                     Prefix(s_People, "Avenida ", "Alameda "),
                     PortugueseRouteNames.NationalRoads)
            };
            for (int i = 0; i < pools.Length; i++)
            {
                var seen = new System.Collections.Generic.HashSet<string>(
                    System.StringComparer.OrdinalIgnoreCase);
                var reviewed = new System.Collections.Generic.List<string>();
                foreach (string name in pools[i])
                {
                    if (!s_ExcludedNames.Contains(name) && seen.Add(name))
                        reviewed.Add(name);
                }
                pools[i] = reviewed.ToArray();
            }
            return pools;
        }

        public PortugueseRoadNameGenerator() : base(
            CreateNamePools(),
            new[]
            {
                "Autoestrada do Norte", "Autoestrada do Sul",
                "Via do Infante", "Autoestrada da Costa de Prata",
                "Autoestrada da Beira Interior", "Autoestrada do Minho",
                "Autoestrada do Douro Litoral", "Autoestrada do Oeste",
                "Autoestrada da Beira Litoral", "Autoestrada de Trás-os-Montes"
            },
            System.Array.Empty<string>(),
            new[]
            {
                "Caminho Rural ", "Travessa ", "Rua ", "Estrada ",
                "Avenida ", "Autoestrada "
            },
            PortugueseRouteNames.Motorways)
        {
        }

        private static string[] Join(params string[][] groups)
        {
            int length = 0;
            foreach (string[] group in groups)
                length += group.Length;
            string[] names = new string[length];
            int offset = 0;
            foreach (string[] group in groups)
            {
                System.Array.Copy(group, 0, names, offset, group.Length);
                offset += group.Length;
            }
            return names;
        }
    }
}
