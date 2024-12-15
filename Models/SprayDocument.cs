namespace SprayMaster.Models
{
    public class SprayDocument
    {
        public string ImagePath { get; set; }
        public List<PaintData> Strokes { get; set; } = new();
    }
}
