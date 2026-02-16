namespace RagazziStudios.Core.Domain.Grid
{
    /// <summary>
    /// Representa uma célula individual do grid.
    /// </summary>
    public class CellData
    {
        public int Row { get; }
        public int Col { get; }
        public char Letter { get; set; }
        public bool IsPartOfWord { get; set; }

        public CellData(int row, int col)
        {
            Row = row;
            Col = col;
            Letter = '\0';
            IsPartOfWord = false;
        }

        public bool IsEmpty => Letter == '\0';

        public override string ToString() => $"[{Row},{Col}]={Letter}";
    }
}
