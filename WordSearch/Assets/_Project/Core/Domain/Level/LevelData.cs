using System.Collections.Generic;
using RagazziStudios.Core.Domain.Grid;

namespace RagazziStudios.Core.Domain.Level
{
    /// <summary>
    /// Dados de um nível gerado.
    /// Contém o grid, as palavras posicionadas e metadados.
    /// </summary>
    public class LevelData
    {
        /// <summary>ID da categoria (ex: "animais").</summary>
        public string CategoryId { get; }

        /// <summary>Número do nível dentro da categoria (1-15).</summary>
        public int LevelNumber { get; }

        /// <summary>Seed usado na geração (para reprodutibilidade).</summary>
        public int Seed { get; }

        /// <summary>Configuração de dificuldade usada.</summary>
        public DifficultyConfig Difficulty { get; }

        /// <summary>O grid gerado.</summary>
        public GridData Grid { get; }

        /// <summary>Palavras posicionadas no grid.</summary>
        public List<WordPlacement> Placements { get; }

        public LevelData(string categoryId, int levelNumber, int seed,
            DifficultyConfig difficulty, GridData grid,
            List<WordPlacement> placements)
        {
            CategoryId = categoryId;
            LevelNumber = levelNumber;
            Seed = seed;
            Difficulty = difficulty;
            Grid = grid;
            Placements = placements;
        }
    }
}
