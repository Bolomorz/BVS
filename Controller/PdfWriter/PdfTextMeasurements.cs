using PdfSharp.Drawing;
using PdfSharp.Pdf.IO;

namespace BVS;

internal class PdfTextMeasurements
{
    internal enum BlockType {Text, Space, Hyphen, LineBreak}

    internal class Block
    {
        public string Text {get; set;}
        public int StartIndex {get; set;}
        public int EndIndex {get; set;}
        public BlockType Type {get; set;}
        public double Width {get; set;}
        public XPoint Location {get; set;}
        public bool Stop {get; set;}

        public Block(string text, BlockType type, double width, int start, int end)
        {
            Text = text;
            Type = type;
            Width = width;
            StartIndex = start;
            EndIndex = end;
        }

        public Block(BlockType type)
        {
            Type = type;
            Text = string.Empty;
        }
    }
    XGraphics Gfx {get; set;}
    string Text {get ; set;}
    XFont Font {get; set;}
    XRect LayoutRectangle {get; set;}
    List<Block> Blocks {get; set;}

    public PdfTextMeasurements(XGraphics gfx, string text, XFont font, XRect rect)
    {
        Gfx = gfx;
        Text = text;
        Font = font;
        LayoutRectangle = rect;
        Blocks = new List<Block>();
    }

    public void MeasureText(out int lastchar, out double neededheight)
    {
        double linespace = Font.GetHeight();
        double cyAscent = linespace * Font.CellAscent / Font.CellSpace;
        double cyDescent = linespace * Font.CellDescent / Font.CellSpace;
        double spaceWidth = Gfx.MeasureString("x x", Font).Width - Gfx.MeasureString("xx", Font).Width;

        lastchar = -1;
        neededheight = double.MinValue;

        CreateBlocks();
        CreateLayout(cyAscent, cyDescent, linespace, spaceWidth);

        double dy = cyDescent + cyAscent;
        int count = Blocks.Count;

        for(int i = 0; i < count; i++)
        {
            var block = Blocks[i];
            if(block.Stop)
            {
                lastchar = 0;
                int j = i - 1;
                while(j >=0)
                {
                    var block2 = Blocks[j];
                    if(block2.EndIndex >= 0)
                    {
                        lastchar = block2.EndIndex;
                        neededheight = dy + block2.Location.Y;
                        return;
                    }
                    --j;
                }
                return;
            }
            if(block.Type == BlockType.LineBreak) continue;
            neededheight = dy + block.Location.Y;
        }
    }
    void CreateBlocks()
    {
        Blocks.Clear();
        int length = Text.Length;
        bool inNonWhiteSpace = false;
        int startIndex = 0;
        int blockLength = 0;

        for(int i = 0; i < length; i++)
        {
            char ch = Text[i];
            if (ch == Chars.CR)
            {
                if (i < length - 1 && Text[i + 1] == Chars.LF)
                ch = Chars.LF;
            }
            if (ch == Chars.LF)
            {
                if(blockLength != 0) Blocks.Add(new Block(Text.Substring(startIndex, blockLength), BlockType.Text, Gfx.MeasureString(Text.Substring(startIndex, blockLength), Font).Width, startIndex, startIndex + blockLength - 1));
                startIndex = i + 1;
                blockLength = 0;
                Blocks.Add(new Block(BlockType.LineBreak));
            }
            else if (char.IsWhiteSpace(ch))
            {
                if (inNonWhiteSpace)
                {
                    Blocks.Add(new Block(Text.Substring(startIndex, blockLength), BlockType.Text, Gfx.MeasureString(Text.Substring(startIndex, blockLength), Font).Width, startIndex, startIndex + blockLength - 1));
                    startIndex = i + 1;
                    blockLength = 0;
                }
                else
                {
                    blockLength++;
                }
            }
            else
            {
                inNonWhiteSpace = true;
                blockLength++;
            }
        }
        if(blockLength != 0)
        {
            Blocks.Add(new Block(Text.Substring(startIndex, blockLength), BlockType.Text, Gfx.MeasureString(Text.Substring(startIndex, blockLength), Font).Width, startIndex, startIndex + blockLength - 1));
        }
    }
    void CreateLayout(double cyAscent, double cyDescent, double linespace, double spacewidth)
    {
        double rwidth = LayoutRectangle.Width;
        double rheight = LayoutRectangle.Height - cyAscent - cyDescent;
        double x = 0;
        double y = 0;
        int count = Blocks.Count;

        for(int i = 0; i < count; i++)
        {
            Block block = Blocks[i];
            if(block.Type == BlockType.LineBreak)
            {
                x = 0;
                y += linespace;
                if(y > rheight)
                {
                    block.Stop = true;
                    break;
                }
            }
            else
            {
                double width = block.Width;
                if((x + width <= rwidth || x == 0) && block.Type != BlockType.LineBreak)
                {
                    block.Location = new XPoint(x, y);
                    x += width + spacewidth;
                }
                else
                {
                    y += linespace;
                    if(y > rheight)
                    {
                        block.Stop = true;
                        break;
                    }
                    block.Location = new XPoint(0, y);
                    x = width + spacewidth;
                }
            }
        }
    }
}