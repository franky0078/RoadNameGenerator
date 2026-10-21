// Road Name Generator
// Copyright (C) 2026 franky0078
// Licensed under the GNU General Public License version 3 or later.

using System;
using System.Collections.Generic;

namespace RoadNameGenerator.Naming
{
    // Brazilian Portuguese, kept separate from the Portugal portfolio.
    // Generated combinations are inspired by Brazil, not a real-world address map.
    // Nature/place endings include articles; personal and city names do not.
    public sealed class BrazilianRoadNameGenerator : RegionalRoadNameGenerator
    {
        private static readonly string[] s_Nature =
        {
            "dos Ipês", "das Acácias", "das Palmeiras",
            "dos Coqueiros", "das Mangueiras", "das Goiabeiras",
            "das Jabuticabeiras", "das Pitangueiras", "dos Cajueiros",
            "dos Jacarandás", "das Araucárias", "das Paineiras",
            "dos Jequitibás", "das Aroeiras", "dos Flamboyants",
            "das Quaresmeiras", "dos Manacás", "das Sibipirunas",
            "dos Jatobás", "dos Buritis", "dos Babaçuais",
            "das Carnaúbas", "das Seringueiras", "das Castanheiras",
            "dos Ingás", "dos Cedros", "dos Pinheiros",
            "dos Eucaliptos", "das Laranjeiras", "dos Limoeiros",
            "das Amendoeiras", "das Figueiras", "das Bananeiras",
            "dos Abacateiros", "dos Maracujás", "dos Pequis",
            "das Guabirobas", "das Macaúbas", "dos Tucumãs",
            "dos Açaizeiros", "das Flores", "das Rosas",
            "das Orquídeas", "das Bromélias", "das Hortênsias",
            "das Margaridas", "dos Girassóis", "dos Lírios",
            "das Violetas", "dos Hibiscos", "das Azaleias",
            "das Begônias", "dos Jasmins", "das Vitórias-Régias",
            "das Alamandas", "dos Antúrios", "das Helicônias",
            "das Sempre-Vivas", "das Samambaias", "dos Bambus",
            "dos Sabiás", "dos Bem-te-vis", "das Araras",
            "dos Tucanos", "dos Beija-Flores", "dos Canários",
            "das Garças", "dos Quero-Queros", "dos Tuiuiús",
            "das Andorinhas", "dos Curiós", "dos Pintassilgos",
            "das Capivaras", "das Lontras", "das Borboletas",
            "dos Vaga-Lumes", "do Cerrado", "da Caatinga",
            "da Mata Atlântica", "do Pantanal", "da Restinga",
            "do Manguezal", "da Floresta", "do Bosque",
            "da Serra", "da Chapada", "do Morro",
            "do Vale", "da Várzea", "da Lagoa",
            "do Lago", "do Rio", "do Riacho",
            "do Córrego", "da Cachoeira", "da Nascente",
            "da Fonte", "das Águas Claras", "das Águas Verdes",
            "da Praia", "das Dunas", "da Baía",
            "do Sol", "da Lua", "das Estrelas",
            "do Horizonte", "da Brisa", "do Luar",
            "do Poente", "da Aurora",
        };

        private static readonly string[] s_Places =
        {
            "da Liberdade", "da Independência", "da República",
            "da Constituição", "da União", "da Paz",
            "da Esperança", "da Alegria", "da Amizade",
            "da Saudade", "da Primavera", "da Harmonia",
            "da Fraternidade", "da Igualdade", "da Solidariedade",
            "do Progresso", "do Trabalho", "da Cultura",
            "da Educação", "da Ciência", "do Comércio",
            "da Indústria", "do Mercado", "da Feira",
            "da Estação", "da Escola", "da Biblioteca",
            "do Teatro", "do Museu", "do Hospital",
            "da Igreja", "da Capela", "do Cruzeiro",
            "da Matriz", "do Rosário", "da Conceição",
            "da Glória", "da Vitória", "do Carmo",
            "do Bom Jesus", "do Divino Espírito Santo", "de Nossa Senhora Aparecida",
            "de Santa Rita", "de Santa Luzia", "de Santa Clara",
            "de Santa Cecília", "de Santa Bárbara", "de São João",
            "de São Pedro", "de São José", "de São Bento",
            "de São Francisco", "de São Sebastião", "de Santo Antônio",
            "dos Pescadores", "dos Agricultores", "dos Trabalhadores",
            "dos Professores", "dos Estudantes", "dos Artesãos",
            "dos Oleiros", "dos Ferreiros", "dos Carpinteiros",
            "das Rendeiras", "das Lavadeiras", "dos Poetas",
            "dos Músicos", "dos Artistas", "dos Esportistas",
            "dos Ferroviários", "dos Navegantes", "dos Imigrantes",
            "dos Bandeirantes", "do Porto", "da Ponte",
            "do Farol", "do Jardim", "do Parque",
            "da Vila Nova", "do Alto da Serra", "do Campo Grande",
            "do Bom Retiro", "da Boa Vista", "do Recanto",
            "do Ribeirão", "do Engenho", "da Fazenda",
            "do Sítio", "do Cafezal", "do Canavial",
            "do Pomar", "da Colônia",
        };

        private static readonly string[] s_People =
        {
            "Machado de Assis", "Carlos Drummond de Andrade", "Cecília Meireles",
            "Clarice Lispector", "Jorge Amado", "Graciliano Ramos",
            "Guimarães Rosa", "Lima Barreto", "Castro Alves",
            "Gonçalves Dias", "José de Alencar", "Manuel Bandeira",
            "Mário de Andrade", "Oswald de Andrade", "Rachel de Queiroz",
            "Cora Coralina", "Carolina Maria de Jesus", "Conceição Evaristo",
            "Lygia Fagundes Telles", "Ana Cristina Cesar", "Adélia Prado",
            "Ruth Rocha", "Lygia Bojunga", "Monteiro Lobato",
            "Érico Veríssimo", "João Cabral de Melo Neto", "Ferreira Gullar",
            "Ariano Suassuna", "Patativa do Assaré", "Euclides da Cunha",
            "Álvares de Azevedo", "Casimiro de Abreu", "Cruz e Sousa",
            "Alphonsus de Guimaraens", "Vinicius de Moraes", "Tom Jobim",
            "Heitor Villa-Lobos", "Chiquinha Gonzaga", "Pixinguinha",
            "Cartola", "Noel Rosa", "Luiz Gonzaga",
            "Dorival Caymmi", "Ary Barroso", "Adoniran Barbosa",
            "Elis Regina", "Clara Nunes", "Dona Ivone Lara",
            "Elza Soares", "Milton Nascimento", "Gilberto Gil",
            "Caetano Veloso", "Chico Buarque", "João Gilberto",
            "Baden Powell", "Jackson do Pandeiro", "Alceu Valença",
            "Tarsila do Amaral", "Anita Malfatti", "Candido Portinari",
            "Di Cavalcanti", "Alfredo Volpi", "Lygia Clark",
            "Lygia Pape", "Hélio Oiticica", "Athos Bulcão",
            "Aleijadinho", "Oscar Niemeyer", "Lúcio Costa",
            "Roberto Burle Marx", "Lina Bo Bardi", "Santos Dumont",
            "Oswaldo Cruz", "Carlos Chagas", "Vital Brazil",
            "Nise da Silveira", "Bertha Lutz", "César Lattes",
            "Mário Schenberg", "Milton Santos", "Josué de Castro",
            "Paulo Freire", "Darcy Ribeiro", "Anísio Teixeira",
            "Rondon", "André Rebouças", "Luiz Gama",
            "José do Patrocínio", "Joaquim Nabuco", "Maria Quitéria",
            "Anita Garibaldi", "Dandara", "Zumbi dos Palmares",
            "Tiradentes", "Chico Mendes", "Dorothy Stang",
            "Ayrton Senna", "Pelé", "Garrincha",
            "Daiane dos Santos", "Maria Lenk", "Adhemar Ferreira da Silva",
        };

        private static readonly string[] s_Geography =
        {
            "Brasil", "Brasília", "São Paulo",
            "Rio de Janeiro", "Belo Horizonte", "Salvador",
            "Recife", "Olinda", "Fortaleza",
            "Natal", "João Pessoa", "Maceió",
            "Aracaju", "Teresina", "São Luís",
            "Belém", "Manaus", "Macapá",
            "Boa Vista", "Porto Velho", "Rio Branco",
            "Palmas", "Goiânia", "Cuiabá",
            "Campo Grande", "Vitória", "Curitiba",
            "Florianópolis", "Porto Alegre", "Campinas",
            "Santos", "Sorocaba", "Ribeirão Preto",
            "São José dos Campos", "Niterói", "Petrópolis",
            "Paraty", "Ouro Preto", "Mariana",
            "Diamantina", "Uberlândia", "Juiz de Fora",
            "Ilhéus", "Porto Seguro", "Feira de Santana",
            "Caruaru", "Petrolina", "Mossoró",
            "Campina Grande", "Sobral", "Juazeiro do Norte",
            "Parnaíba", "Imperatriz", "Santarém",
            "Marabá", "Parintins", "Joinville",
            "Blumenau", "Londrina", "Maringá",
            "Foz do Iguaçu", "Caxias do Sul", "Pelotas",
            "Gramado", "Acre", "Alagoas",
            "Amapá", "Amazonas", "Bahia",
            "Ceará", "Espírito Santo", "Goiás",
            "Maranhão", "Mato Grosso", "Mato Grosso do Sul",
            "Minas Gerais", "Pará", "Paraíba",
            "Paraná", "Pernambuco", "Piauí",
            "Rio Grande do Norte", "Rio Grande do Sul", "Rondônia",
            "Roraima", "Santa Catarina", "Sergipe",
            "Tocantins",
        };

        private static readonly string[] s_Dates =
        {
            "Sete de Setembro", "Quinze de Novembro", "Vinte e Um de Abril",
            "Primeiro de Maio", "Vinte de Novembro", "Dois de Julho",
            "Treze de Maio", "Vinte e Cinco de Janeiro", "Nove de Julho",
            "Doze de Outubro",
        };

        private static readonly string[] s_Rural =
        {
            "da Fazenda Velha", "da Fazenda Nova", "do Sítio São José",
            "do Sítio Boa Vista", "da Colônia Nova", "do Engenho Velho",
            "do Alto do Morro", "do Córrego Fundo", "do Ribeirão das Pedras",
            "da Cachoeira Grande", "do Rio das Antas", "do Rio dos Peixes",
            "do Vale Verde", "do Campo Alegre", "do Capão",
            "da Várzea Grande", "da Serra Azul", "do Morro Alto",
            "da Pedra Branca", "do Barreiro", "do Brejo",
            "da Mata dos Cocais", "do Sertão", "da Roça",
            "do Cafezal", "dos Canaviais", "dos Laranjais",
            "dos Arrozais", "das Pastagens", "dos Tropeiros",
        };

        private static readonly string[] s_Routes =
        {
            "BR-010", "BR-020", "BR-030",
            "BR-040", "BR-050", "BR-060",
            "BR-070", "BR-080", "BR-101",
            "BR-104", "BR-110", "BR-116",
            "BR-122", "BR-135", "BR-153",
            "BR-158", "BR-163", "BR-174",
            "BR-210", "BR-222", "BR-226",
            "BR-230", "BR-232", "BR-235",
            "BR-242", "BR-251", "BR-262",
            "BR-267", "BR-277", "BR-280",
            "BR-282", "BR-285", "BR-290",
            "BR-293", "BR-304", "BR-316",
            "BR-319", "BR-324", "BR-330",
            "BR-342", "BR-343", "BR-349",
            "BR-354", "BR-356", "BR-364",
            "BR-365", "BR-369", "BR-376",
            "BR-381", "BR-386", "BR-392",
            "BR-393", "BR-401", "BR-407",
            "BR-470", "BR-488", "SP-021",
            "SP-065", "SP-070", "SP-075",
            "SP-150", "SP-160", "SP-270",
            "SP-280", "SP-330", "SP-348",
        };

        private static readonly string[] s_Highways =
        {
            "Rodovia Presidente Dutra", "Rodovia Fernão Dias", "Rodovia Régis Bittencourt",
            "Rodovia Transamazônica", "Rodovia dos Bandeirantes", "Rodovia dos Imigrantes",
            "Rodovia Anchieta", "Rodovia Anhanguera", "Rodovia Ayrton Senna",
            "Rodovia Carvalho Pinto", "Rodovia Raposo Tavares", "Rodovia Castello Branco",
            "Rodovia Dom Pedro I", "Rodovia dos Ipês", "Rodovia das Palmeiras",
            "Rodovia da Serra Azul", "Rodovia do Vale Verde", "Rodovia do Litoral",
            "Rodovia da Integração", "Rodovia do Cerrado", "Rodovia das Araucárias",
            "Rodovia dos Coqueiros", "Rodovia da Mantiqueira", "Rodovia da Serra do Mar",
            "Rodovia do Sol", "Rodovia da Costa Verde", "Rodovia dos Pioneiros",
            "Rodovia dos Tropeiros", "Rodovia das Cachoeiras", "Rodovia do Pantanal",
        };

        public BrazilianRoadNameGenerator() : base(
            new[]
            {
                Join(Prefix(s_Rural, "Estrada ", "Caminho "),
                     Prefix(s_Nature, "Estrada ")),
                Join(Prefix(s_Nature, "Travessa ", "Beco "),
                     Prefix(s_Places, "Travessa ", "Beco "),
                     Prefix(s_People, "Travessa ")),
                Join(Prefix(s_Nature, "Rua ", "Alameda "),
                     Prefix(s_Places, "Rua "), Prefix(s_People, "Rua "),
                     Prefix(s_Geography, "Rua "), Prefix(s_Dates, "Rua ")),
                Join(Prefix(s_Nature, "Rua "), Prefix(s_Places, "Rua "),
                     Prefix(s_People, "Rua "), Prefix(s_Geography, "Rua "),
                     Prefix(s_Dates, "Rua "), Prefix(s_Rural, "Estrada ")),
                Join(Prefix(s_Nature, "Avenida "), Prefix(s_Places, "Avenida "),
                     Prefix(s_People, "Avenida "), Prefix(s_Geography, "Avenida "),
                     Prefix(s_Dates, "Avenida "))
            },
            s_Highways,
            Array.Empty<string>(),
            new[] { "Estrada Rural ", "Travessa ", "Rua ", "Rua ", "Avenida ", "Rodovia " },
            s_Routes)
        {
        }

        private static string[] Join(params string[][] groups)
        {
            var names = new List<string>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string[] group in groups)
                foreach (string name in group)
                    if (seen.Add(name))
                        names.Add(name);
            return names.ToArray();
        }
    }
}
