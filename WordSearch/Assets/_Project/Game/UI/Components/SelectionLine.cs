using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using RagazziStudios.Core.Domain.Grid;

namespace RagazziStudios.Game.UI.Components
{
    /// <summary>
    /// Gerencia a seleção por arrasto (touch/mouse) no grid de letras.
    /// Detecta o gesto linear e informa as posições selecionadas.
    /// Suporta seleção horizontal, vertical e diagonal.
    /// Mostra linhas persistentes coloridas sobre palavras encontradas.
    /// </summary>
    public class SelectionLine : MonoBehaviour, IPointerDownHandler,
        IPointerUpHandler, IDragHandler
    {
        [Header("Referências")]
        [SerializeField] private GridView _gridView;
        [SerializeField] private RectTransform _lineVisual;

        [Header("Visual")]
        [SerializeField] private Image _lineImage;
        [SerializeField] private Color _lineColor = new Color(0.4f, 0.7f, 1f, 0.6f);
        [SerializeField] private float _lineThickness = 40f;

        /// <summary>Sprite procedural em forma de pílula (endpoints arredondados).</summary>
        private Sprite _pillSprite;

        /// <summary>Contador de palavras encontradas (para ciclar cores).</summary>
        private int _foundCount;

        /// <summary>Paleta de cores para linhas de palavras encontradas.</summary>
        private static readonly Color[] FoundColors = new Color[]
        {
            new Color(0.35f, 0.78f, 0.35f, 0.50f), // verde
            new Color(0.90f, 0.55f, 0.20f, 0.50f), // laranja
            new Color(0.70f, 0.35f, 0.75f, 0.50f), // roxo
            new Color(0.90f, 0.35f, 0.35f, 0.50f), // vermelho
            new Color(0.35f, 0.75f, 0.85f, 0.50f), // ciano
            new Color(0.90f, 0.80f, 0.20f, 0.50f), // amarelo
        };

        /// <summary>Posições atualmente selecionadas (row, col).</summary>
        private readonly List<(int row, int col)> _selectedPositions = new List<(int, int)>();

        /// <summary>Célula onde o toque começou.</summary>
        private LetterCell _startCell;

        /// <summary>Célula atual sob o dedo.</summary>
        private LetterCell _currentCell;

        /// <summary>Se há uma seleção ativa.</summary>
        public bool IsSelecting { get; private set; }

        /// <summary>Posições selecionadas (somente leitura).</summary>
        public IReadOnlyList<(int row, int col)> SelectedPositions => _selectedPositions;

        /// <summary>Callback quando a seleção é finalizada.</summary>
        public event System.Action<IReadOnlyList<(int row, int col)>> OnSelectionComplete;

        private void Start()
        {
            _pillSprite = CreatePillSprite();

            if (_lineVisual != null)
                _lineVisual.gameObject.SetActive(false);

            if (_lineImage != null)
            {
                _lineImage.color = _lineColor;
                _lineImage.sprite = _pillSprite;
                _lineImage.type = Image.Type.Sliced;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            var cell = GetCellAtPosition(eventData.position);
            if (cell == null) return;

            IsSelecting = true;
            _startCell = cell;
            _currentCell = cell;

            UpdateSelection(cell);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!IsSelecting) return;

            var cell = GetCellAtPosition(eventData.position);
            if (cell == null || cell == _currentCell) return;

            _currentCell = cell;
            UpdateLinearSelection(_startCell, _currentCell);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!IsSelecting) return;

            IsSelecting = false;

            // Notificar que a seleção foi finalizada
            if (_selectedPositions.Count > 0)
            {
                OnSelectionComplete?.Invoke(_selectedPositions);
            }

            ClearVisualSelection();
        }

        /// <summary>
        /// Atualiza a seleção para uma única célula.
        /// </summary>
        private void UpdateSelection(LetterCell cell)
        {
            ClearVisualSelection();

            _selectedPositions.Clear();
            _selectedPositions.Add((cell.Row, cell.Col));

            cell.SetState(CellState.Selected);
            UpdateLineVisual();
        }

        /// <summary>
        /// Calcula a seleção linear entre startCell e endCell.
        /// Só permite linhas retas (horizontal, vertical, diagonal).
        /// </summary>
        private void UpdateLinearSelection(LetterCell startCell, LetterCell endCell)
        {
            // Limpar seleção visual anterior
            _gridView.ClearSelection();
            _selectedPositions.Clear();

            int dRow = endCell.Row - startCell.Row;
            int dCol = endCell.Col - startCell.Col;

            // Determinar se é uma linha válida
            int absDRow = Mathf.Abs(dRow);
            int absDCol = Mathf.Abs(dCol);

            bool isHorizontal = dRow == 0 && dCol != 0;
            bool isVertical = dCol == 0 && dRow != 0;
            bool isDiagonal = absDRow == absDCol && absDRow > 0;

            if (!isHorizontal && !isVertical && !isDiagonal)
            {
                // Seleção inválida — manter apenas start
                _selectedPositions.Add((startCell.Row, startCell.Col));
                startCell.SetState(CellState.Selected);
                UpdateLineVisual();
                return;
            }

            // Calcular step direction
            int stepRow = dRow == 0 ? 0 : (dRow > 0 ? 1 : -1);
            int stepCol = dCol == 0 ? 0 : (dCol > 0 ? 1 : -1);
            int steps = Mathf.Max(absDRow, absDCol);

            // Construir lista de posições
            for (int i = 0; i <= steps; i++)
            {
                int r = startCell.Row + i * stepRow;
                int c = startCell.Col + i * stepCol;

                _selectedPositions.Add((r, c));

                var cell = _gridView.GetCell(r, c);
                cell?.SetState(CellState.Selected);
            }

            UpdateLineVisual();
        }

        /// <summary>
        /// Limpa o visual da seleção atual.
        /// </summary>
        private void ClearVisualSelection()
        {
            _gridView?.ClearSelection();
            _selectedPositions.Clear();

            if (_lineVisual != null)
                _lineVisual.gameObject.SetActive(false);
        }

        /// <summary>
        /// Atualiza a posição e tamanho da linha visual entre primeira e última célula.
        /// </summary>
        private void UpdateLineVisual()
        {
            if (_lineVisual == null || _selectedPositions.Count < 2)
            {
                if (_lineVisual != null)
                    _lineVisual.gameObject.SetActive(false);
                return;
            }

            var first = _selectedPositions[0];
            var last = _selectedPositions[_selectedPositions.Count - 1];

            var firstCell = _gridView.GetCell(first.row, first.col);
            var lastCell = _gridView.GetCell(last.row, last.col);

            if (firstCell == null || lastCell == null) return;

            // Posicionar a linha entre os centros das células
            var firstRT = firstCell.GetComponent<RectTransform>();
            var lastRT = lastCell.GetComponent<RectTransform>();

            Vector3 startPos = firstRT.position;
            Vector3 endPos = lastRT.position;
            Vector3 midPos = (startPos + endPos) / 2f;

            _lineVisual.position = midPos;

            // Calcular rotação
            Vector3 diff = endPos - startPos;
            float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            _lineVisual.rotation = Quaternion.Euler(0, 0, angle);

            // Calcular comprimento
            float distance = diff.magnitude;
            _lineVisual.sizeDelta = new Vector2(distance + _lineThickness * 0.5f, _lineThickness);

            _lineVisual.gameObject.SetActive(true);
        }

        /// <summary>
        /// Encontra qual LetterCell está sob a posição de toque.
        /// </summary>
        private LetterCell GetCellAtPosition(Vector2 screenPosition)
        {
            if (_gridView?.Cells == null) return null;

            // Raycast na UI
            var results = new List<RaycastResult>();
            var pointerData = new PointerEventData(EventSystem.current)
            {
                position = screenPosition
            };
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (var result in results)
            {
                var cell = result.gameObject.GetComponent<LetterCell>();
                if (cell != null) return cell;
            }

            return null;
        }

        // ═══════════════════════════════════════════════════════════════
        //  Linhas persistentes para palavras encontradas
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Cria uma linha colorida persistente sobre a palavra encontrada.
        /// Chamada pelo GameplayController quando uma palavra é validada.
        /// </summary>
        public void ShowFoundLine(WordPlacement placement)
        {
            if (_gridView == null || _pillSprite == null) return;

            var positions = placement.GetCellPositions();
            if (positions.Length < 2) return;

            var firstPos = positions[0];
            var lastPos = positions[positions.Length - 1];

            var firstCell = _gridView.GetCell(firstPos.row, firstPos.col);
            var lastCell = _gridView.GetCell(lastPos.row, lastPos.col);
            if (firstCell == null || lastCell == null) return;

            // Criar GO da linha
            var lineGO = new GameObject($"FoundLine_{placement.DisplayWord}");
            lineGO.transform.SetParent(transform, false);

            var img = lineGO.AddComponent<Image>();
            img.sprite = _pillSprite;
            img.type = Image.Type.Sliced;
            img.color = FoundColors[_foundCount % FoundColors.Length];
            img.raycastTarget = false;

            // Posicionar entre os centros da primeira e última célula
            var firstRT = firstCell.GetComponent<RectTransform>();
            var lastRT = lastCell.GetComponent<RectTransform>();

            Vector3 startPos = firstRT.position;
            Vector3 endPos = lastRT.position;

            var rt = lineGO.GetComponent<RectTransform>();
            rt.position = (startPos + endPos) / 2f;

            Vector3 diff = endPos - startPos;
            float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            rt.rotation = Quaternion.Euler(0, 0, angle);

            float distance = diff.magnitude;
            rt.sizeDelta = new Vector2(distance + _lineThickness, _lineThickness);

            _foundCount++;
        }

        /// <summary>Remove todas as linhas de palavras encontradas (ex: reiniciar nível).</summary>
        public void ClearFoundLines()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
                if (child != _lineVisual)
                    Destroy(child.gameObject);
            }
            _foundCount = 0;
        }

        // ═══════════════════════════════════════════════════════════════
        //  Sprite procedural (pílula com endpoints arredondados)
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Gera um Sprite em forma de pílula (retângulo com extremidades semicirculares).
        /// Usa 9-slice para que as pontas arredondadas não distorçam ao esticar.
        /// </summary>
        private static Sprite CreatePillSprite()
        {
            const int w = 128, h = 64;
            float r = h * 0.5f;

            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            var pixels = new Color32[w * h];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float px = x + 0.5f;
                    float py = y + 0.5f;
                    float cy = h * 0.5f;

                    float dist;

                    if (px < r)
                    {
                        // Extremidade esquerda (semicírculo)
                        float dx = px - r;
                        float dy = py - cy;
                        dist = r - Mathf.Sqrt(dx * dx + dy * dy);
                    }
                    else if (px > w - r)
                    {
                        // Extremidade direita (semicírculo)
                        float dx = px - (w - r);
                        float dy = py - cy;
                        dist = r - Mathf.Sqrt(dx * dx + dy * dy);
                    }
                    else
                    {
                        // Meio (retângulo)
                        dist = Mathf.Min(py, h - py);
                    }

                    // Anti-alias suave (transição de 1.5px)
                    float alpha = Mathf.Clamp01(dist / 1.5f + 0.5f);
                    byte a = (byte)(alpha * 255);
                    pixels[y * w + x] = new Color32(255, 255, 255, a);
                }
            }

            tex.SetPixels32(pixels);
            tex.Apply();

            // 9-slice: preservar extremidades arredondadas (left=r, bottom=0, right=r, top=0)
            var border = new Vector4(r, 0, r, 0);
            return Sprite.Create(tex, new Rect(0, 0, w, h),
                new Vector2(0.5f, 0.5f), 100, 0,
                SpriteMeshType.FullRect, border);
        }
    }
}
