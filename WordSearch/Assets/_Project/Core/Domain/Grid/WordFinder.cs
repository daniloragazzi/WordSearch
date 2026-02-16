using System;
using System.Collections.Generic;

namespace RagazziStudios.Core.Domain.Grid
{
    /// <summary>
    /// Valida seleções do jogador contra as palavras posicionadas no grid.
    /// Classe pura C# — sem dependência de Unity.
    /// </summary>
    public class WordFinder
    {
        private readonly List<WordPlacement> _placements;

        /// <summary>Evento disparado quando uma palavra é encontrada.</summary>
        public event Action<WordPlacement> OnWordFound;

        /// <summary>Evento disparado quando todas as palavras foram encontradas.</summary>
        public event Action OnAllWordsFound;

        public WordFinder(List<WordPlacement> placements)
        {
            _placements = placements ?? throw new ArgumentNullException(nameof(placements));
        }

        /// <summary>
        /// Total de palavras no nível.
        /// </summary>
        public int TotalWords => _placements.Count;

        /// <summary>
        /// Quantidade de palavras já encontradas.
        /// </summary>
        public int FoundCount
        {
            get
            {
                int count = 0;
                foreach (var p in _placements)
                {
                    if (p.Found) count++;
                }
                return count;
            }
        }

        /// <summary>
        /// Todas as palavras foram encontradas?
        /// </summary>
        public bool AllFound => FoundCount == TotalWords;

        /// <summary>
        /// Retorna a lista de todas as palavras (para exibir na UI).
        /// </summary>
        public IReadOnlyList<WordPlacement> Placements => _placements.AsReadOnly();

        /// <summary>
        /// Verifica se a sequência de posições forma uma palavra válida.
        /// Se sim, marca como encontrada e dispara evento.
        /// </summary>
        /// <param name="selectedPositions">Posições selecionadas pelo jogador (row, col).</param>
        /// <returns>O WordPlacement encontrado, ou null se não é uma palavra válida.</returns>
        public WordPlacement CheckSelection(IReadOnlyList<(int row, int col)> selectedPositions)
        {
            if (selectedPositions == null || selectedPositions.Count == 0)
                return null;

            foreach (var placement in _placements)
            {
                if (placement.Found)
                    continue;

                if (MatchesPlacement(placement, selectedPositions))
                {
                    placement.Found = true;
                    OnWordFound?.Invoke(placement);

                    if (AllFound)
                    {
                        OnAllWordsFound?.Invoke();
                    }

                    return placement;
                }
            }

            return null;
        }

        /// <summary>
        /// Retorna uma palavra não encontrada aleatória (para dica).
        /// </summary>
        public WordPlacement GetHint(Random random = null)
        {
            var unfound = new List<WordPlacement>();
            foreach (var p in _placements)
            {
                if (!p.Found) unfound.Add(p);
            }

            if (unfound.Count == 0)
                return null;

            random = random ?? new Random();
            return unfound[random.Next(unfound.Count)];
        }

        /// <summary>
        /// Revela uma palavra (marca como encontrada) — usado para dica.
        /// </summary>
        public void RevealWord(WordPlacement placement)
        {
            if (placement == null || placement.Found)
                return;

            placement.Found = true;
            OnWordFound?.Invoke(placement);

            if (AllFound)
            {
                OnAllWordsFound?.Invoke();
            }
        }

        /// <summary>
        /// Verifica se a seleção corresponde exatamente às posições de um placement.
        /// Aceita seleção em ambas as direções (início→fim ou fim→início).
        /// </summary>
        private bool MatchesPlacement(WordPlacement placement,
            IReadOnlyList<(int row, int col)> selectedPositions)
        {
            var expected = placement.GetCellPositions();

            if (selectedPositions.Count != expected.Length)
                return false;

            // Verificar direção normal
            if (MatchesForward(expected, selectedPositions))
                return true;

            // Verificar direção reversa (jogador arrastou ao contrário)
            if (MatchesReverse(expected, selectedPositions))
                return true;

            return false;
        }

        private bool MatchesForward((int row, int col)[] expected,
            IReadOnlyList<(int row, int col)> selected)
        {
            for (int i = 0; i < expected.Length; i++)
            {
                if (expected[i].row != selected[i].row ||
                    expected[i].col != selected[i].col)
                    return false;
            }
            return true;
        }

        private bool MatchesReverse((int row, int col)[] expected,
            IReadOnlyList<(int row, int col)> selected)
        {
            int last = expected.Length - 1;
            for (int i = 0; i < expected.Length; i++)
            {
                if (expected[last - i].row != selected[i].row ||
                    expected[last - i].col != selected[i].col)
                    return false;
            }
            return true;
        }
    }
}
