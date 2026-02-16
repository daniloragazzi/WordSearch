using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RagazziStudios.Core.Domain.Grid;

namespace RagazziStudios.Game.UI.Components
{
    /// <summary>
    /// Renderiza o grid de letras na UI.
    /// Instancia LetterCells em um GridLayoutGroup.
    /// </summary>
    public class GridView : MonoBehaviour
    {
        [Header("Referências")]
        [SerializeField] private GameObject _letterCellPrefab;
        [SerializeField] private GridLayoutGroup _gridLayout;
        [SerializeField] private RectTransform _gridContainer;

        [Header("Configuração")]
        [SerializeField] private float _cellSpacing = 4f;
        [SerializeField] private float _maxGridSize = 700f;

        private LetterCell[,] _cells;
        private int _rows;
        private int _cols;

        /// <summary>Todas as células criadas (acesso por [row, col]).</summary>
        public LetterCell[,] Cells => _cells;
        public int Rows => _rows;
        public int Cols => _cols;

        /// <summary>
        /// Cria o grid visual a partir dos dados do grid lógico.
        /// </summary>
        public void BuildGrid(GridData gridData)
        {
            ClearGrid();

            _rows = gridData.Rows;
            _cols = gridData.Cols;
            _cells = new LetterCell[_rows, _cols];

            // Forçar atualização do layout para obter dimensões reais do container
            Canvas.ForceUpdateCanvases();

            // Usar tamanho real do container (stretch anchors) em vez de valor fixo
            float containerWidth = _gridContainer != null ? _gridContainer.rect.width : 0;
            float containerHeight = _gridContainer != null ? _gridContainer.rect.height : 0;

            // Fallback para _maxGridSize se o rect ainda não foi computado
            if (containerWidth <= 0) containerWidth = _maxGridSize;
            if (containerHeight <= 0) containerHeight = _maxGridSize;

            // Reduzir espaçamento dinamicamente para grids muito densos
            float spacing = _cellSpacing;
            int maxDim = Mathf.Max(_rows, _cols);
            if (maxDim > 16) spacing = Mathf.Max(1f, _cellSpacing * (16f / maxDim));

            float availableWidth = containerWidth - (spacing * (_cols - 1));
            float availableHeight = containerHeight - (spacing * (_rows - 1));
            float cellSize = Mathf.Min(availableWidth / _cols, availableHeight / _rows);
            cellSize = Mathf.Floor(cellSize); // Pixel-perfect

            // Configurar GridLayoutGroup
            if (_gridLayout != null)
            {
                _gridLayout.cellSize = new Vector2(cellSize, cellSize);
                _gridLayout.spacing = new Vector2(spacing, spacing);
                _gridLayout.constraintCount = _cols;
                _gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            }

            // Não modificar _gridContainer.sizeDelta em modo stretch.
            // O GridLayoutGroup com childAlignment = MiddleCenter centraliza o conteúdo.

            // Instanciar células
            for (int r = 0; r < _rows; r++)
            {
                for (int c = 0; c < _cols; c++)
                {
                    var cellObj = Instantiate(_letterCellPrefab, _gridLayout.transform);
                    cellObj.SetActive(true);
                    var cell = cellObj.GetComponent<LetterCell>();

                    if (cell != null)
                    {
                        char letter = gridData.GetLetter(r, c);
                        cell.Setup(r, c, letter);
                        _cells[r, c] = cell;
                    }
                }
            }
        }

        /// <summary>
        /// Retorna a LetterCell na posição informada, ou null.
        /// </summary>
        public LetterCell GetCell(int row, int col)
        {
            if (_cells == null) return null;
            if (row < 0 || row >= _rows || col < 0 || col >= _cols) return null;
            return _cells[row, col];
        }

        /// <summary>
        /// Marca as células de uma palavra como encontradas.
        /// </summary>
        public void MarkWordFound(WordPlacement placement)
        {
            var positions = placement.GetCellPositions();
            foreach (var (row, col) in positions)
            {
                var cell = GetCell(row, col);
                cell?.SetState(CellState.Found);
            }
        }

        /// <summary>
        /// Destaca as células de uma palavra como dica.
        /// </summary>
        public void HighlightHint(WordPlacement placement)
        {
            var positions = placement.GetCellPositions();
            foreach (var (row, col) in positions)
            {
                var cell = GetCell(row, col);
                cell?.SetState(CellState.Hint);
            }
        }

        /// <summary>
        /// Remove highlight de seleção de todas as células não encontradas.
        /// </summary>
        public void ClearSelection()
        {
            if (_cells == null) return;

            for (int r = 0; r < _rows; r++)
            {
                for (int c = 0; c < _cols; c++)
                {
                    var cell = _cells[r, c];
                    if (cell != null && cell.IsSelected)
                    {
                        cell.SetState(CellState.Normal);
                    }
                }
            }
        }

        /// <summary>
        /// Destrói todas as células do grid.
        /// </summary>
        public void ClearGrid()
        {
            if (_gridLayout == null) return;

            foreach (Transform child in _gridLayout.transform)
            {
                Destroy(child.gameObject);
            }

            _cells = null;
            _rows = 0;
            _cols = 0;
        }
    }
}
