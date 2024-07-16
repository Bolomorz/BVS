using System.Reflection.Metadata.Ecma335;
using Microsoft.VisualBasic;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;

namespace BVS;

internal enum PdfFileType {Document, Template}

internal class PdfDocumentObject
{
    internal required PdfDocument Document { get; set; }
    internal required List<PdfPage> Pages { get; set; }
    internal required List<XGraphics> Gfxs { get; set; }
    internal required int CurrentIndex { get; set; } = -1;
    internal required PageSize PageSize { get; set; }
    internal required PageOrientation PageOrientation { get; set; } 
    internal void AddPage()
    {
        var newpage = Document.AddPage();
        newpage.Size = PageSize;
        newpage.Orientation = PageOrientation;
        var gfx = XGraphics.FromPdfPage(newpage);
        Pages.Add(newpage);
        Gfxs.Add(gfx);
        CurrentIndex++;
    }
}

internal class PdfDrawInformation
{
    internal required bool PreviousSuccess { get; set; }
    internal required double LastVerticalPosition { get; set; }
    internal required double VerticalStart { get; set; }
    internal required double VerticalEnd { get; set; }
    internal required double HorizontalStart { get; set; }
    internal required double HorizontalEnd { get; set; }
}

internal class PdfTemplateObject
{
    internal required PdfTemplate Template { get; set; }
    internal required List<PdfTemplateDynamicItemCollection> DynamicItemCollections { get; set; }
    internal string? GetKeyValueFromTemplate(string key)
    {
        switch(key)
        {
            case "TemplateName": return Template.Name;
            default: return null;
        }
    }
    internal PdfDocument? Draw(PdfFileType type, PdfData data)
    {
        if(Template.Name is null) {Logging.WriteLog("nv.template.name", "Value of DBObject is null.", Template.PdfTemplateID); return null;}
        if(Template.VerticalStart is null) {Logging.WriteLog("nv.template.verticalstart", "Value of DBObject is null.", Template.PdfTemplateID); return null;}
        if(Template.VerticalEnd is null) {Logging.WriteLog("nv.template.verticalend", "Value of DBObject is null.", Template.PdfTemplateID); return null;}
        if(Template.HorizontalStart is null) {Logging.WriteLog("nv.template.horizontalstart", "Value of DBObject is null.", Template.PdfTemplateID); return null;}
        if(Template.HorizontalEnd is null) {Logging.WriteLog("nv.template.horizontalend", "Value of DBObject is null.", Template.PdfTemplateID); return null;}
        if(Template.Orientation is null) {Logging.WriteLog("nv.template.orientation", "Value of DBObject is null.", Template.PdfTemplateID); return null;}
        
        PdfDocument document = new();
        PdfDocumentObject pdo = new()
        {
            Document = document, 
            Pages = new List<PdfPage>(), 
            Gfxs = new List<XGraphics>(), 
            CurrentIndex = -1,
            PageSize = PageSize.A4,
            PageOrientation = (Template.Orientation == "Landscape") ? PageOrientation.Landscape : PageOrientation.Portrait
        };
        pdo.AddPage();

        PdfDrawInformation pdi = new PdfDrawInformation()
        {
            PreviousSuccess = true,
            LastVerticalPosition = (double)Template.VerticalStart,
            VerticalStart = (double)Template.VerticalStart,
            VerticalEnd = (double)Template.VerticalEnd,
            HorizontalStart = (double)Template.HorizontalStart,
            HorizontalEnd = (double)Template.HorizontalEnd,
        };

        foreach(var ic in DynamicItemCollections)
        {
            if(pdi.PreviousSuccess) ic.Draw(pdo, type, data, pdi);
        }

        return pdi.PreviousSuccess ? pdo.Document : null;
    }
}

internal class PdfTemplateDynamicItemCollection
{
    internal required PdfTemplateDynamicItem Item { get; set; }
    internal required List<ITemplateItem> Items { get; set; }

    internal void Draw(PdfDocumentObject pdo, PdfFileType type, PdfData data, PdfDrawInformation pdi)
    {
        if(Item.VerticalOffsetToPreviousItem is null) {Logging.WriteLog("nv.templateitem.offset", "Value of DBObject is null.", Item.PdfTemplateDynamicItemID); pdi.PreviousSuccess = false; return;}
        if(Item.AlwaysStartNewPage is null) {Logging.WriteLog("nv.templateitem.newpage", "Value of DBObject is null.", Item.PdfTemplateDynamicItemID); pdi.PreviousSuccess = false; return;}
        if(Item.RepeatForEachDataObject is null) {Logging.WriteLog("nv.templateitem.repeat", "Value of DBObject is null.", Item.PdfTemplateDynamicItemID); pdi.PreviousSuccess = false; return;}
        
        double newpos;
        if((bool)Item.AlwaysStartNewPage) 
        {
            pdo.AddPage();
            newpos = pdi.VerticalStart;
        }
        else
        {
            newpos = pdi.LastVerticalPosition + (double)Item.VerticalOffsetToPreviousItem;
        }

        if((bool)Item.RepeatForEachDataObject)
        {
            foreach(var d in data.PdfCustomerDatas)
            {
                double need = 0;
                foreach(var item in Items)
                {
                    var mnh = item.PrepareDraw(pdo, type, d, pdi, newpos, pdi.HorizontalStart);
                    if(!mnh.success) {pdi.PreviousSuccess = false; return;}
                    if(mnh.height > need) need = mnh.height;
                }
                bool newpage = false;
                double heightoffset = 0;
                if(newpos + need > pdi.VerticalEnd) 
                {
                    pdo.AddPage();
                    heightoffset = newpos - pdi.VerticalStart;
                    newpage = true;
                }
                foreach(var item in Items)
                {
                    item.DrawItem(pdo, newpage, heightoffset);
                }
            }
        }
        else
        {
            var d = data.PdfCustomerDatas[0];
            double need = 0;
            foreach(var item in Items)
            {
                var mnh = item.PrepareDraw(pdo, type, d, pdi, newpos, pdi.HorizontalStart);
                if(!mnh.success) {pdi.PreviousSuccess = false; return;}
                if(mnh.height > need) need = mnh.height;
            }
            bool newpage = false;
            double heightoffset = 0;
            if(newpos + need > pdi.VerticalEnd) 
            {
                pdo.AddPage();
                heightoffset = newpos - pdi.VerticalStart;
                newpage = true;
            }
            foreach(var item in Items)
            {
                item.DrawItem(pdo, newpage, heightoffset);
            }
        }
    }
}

internal interface ITemplateItem
{
    (double height, bool success) PrepareDraw(PdfDocumentObject pdo, PdfFileType type, PdfCustomerData data, PdfDrawInformation pdi, double top, double left);
    void DrawItem(PdfDocumentObject pdo, bool newpage, double heightoffset);
}

internal class PdfStringItem : ITemplateItem
{
    internal class PdfKeyItem
    {
        internal required PdfKey Key { get; set; }
        internal required PdfTemplateKeyValue TemplateKeyValue { get; set; }
    }
    internal required PdfTemplateString FormatString { get; set; }
    internal required List<PdfKeyItem> Keys { get; set; }

    private bool prepared { get; set; } = false;
    private StringInformation? Info { get; set; }
    private class StringInformation
    {
        internal required string FullString { get; set; }
        internal required double Top { get; set; }
        internal required double Left { get; set; }
        internal required double Right { get; set; }
        internal required double Bottom { get; set; }
        internal required XBrush Brush { get; set; }
        internal required XFont Font { get; set; } 
    }

    public (double height, bool success) PrepareDraw(PdfDocumentObject pdo, PdfFileType type, PdfCustomerData data, PdfDrawInformation pdi, double top, double left)
    {
        if(FormatString.FormatString is null) {Logging.WriteLog("nv.formatstring.formatstring", "Value of DBObject is null.", FormatString.PdfTemplateStringID); return (0, false);} 
        if(FormatString.VerticalStart is null) {Logging.WriteLog("nv.formatstring.verticalstart", "Value of DBObject is null.", FormatString.PdfTemplateStringID); return (0, false);} 
        if(FormatString.HorizontalStart is null) {Logging.WriteLog("nv.formatstring.horizontalstart", "Value of DBObject is null.", FormatString.PdfTemplateStringID); return (0, false);} 
        if(FormatString.HorizontalEnd is null) {Logging.WriteLog("nv.formatstring.horizontalend", "Value of DBObject is null.", FormatString.PdfTemplateStringID); return (0, false);} 
        if(FormatString.Brush is null) {Logging.WriteLog("nv.formatstring.brush", "Value of DBObject is null.", FormatString.PdfTemplateStringID); return (0, false);} 
        if(FormatString.FontFamily is null) {Logging.WriteLog("nv.formatstring.fontfamily", "Value of DBObject is null.", FormatString.PdfTemplateStringID); return (0, false);} 
        if(FormatString.FontSize is null) {Logging.WriteLog("nv.formatstring.fontsize", "Value of DBObject is null.", FormatString.PdfTemplateStringID); return (0, false);} 

        var full = (type == PdfFileType.Document) ? GetFullString(data.Data) : GetTemplateString();
        if(full is null) {Logging.WriteLog("nv.fullstring", "Error writing string from keys.", FormatString.PdfTemplateStringID); return (0, false);}
        var font = new XFont(FormatString.FontFamily, (double)FormatString.FontSize);
        var brush = new XSolidBrush(XColor.FromKnownColor((XKnownColor)FormatString.Brush));
        if(font is null) {Logging.WriteLog("nv.font", "Error reading font", FormatString.PdfTemplateStringID); return (0, false);}
        if(brush is null) {Logging.WriteLog("nv.brush", "Error reading brush", FormatString.PdfTemplateStringID); return (0, false);}

        XRect rect = new(new XPoint((double)FormatString.HorizontalStart, 0), new XPoint((double)FormatString.HorizontalEnd, 800));
        PdfTextMeasurements ptm = new(pdo.Gfxs[pdo.CurrentIndex], full, font, rect);
        int l; double h;
        ptm.MeasureText(out l, out h);

        Info = new()
        {
            FullString = full,
            Top = (double)FormatString.VerticalStart + top,
            Bottom = (double)FormatString.VerticalStart + top + h,
            Left = (double)FormatString.HorizontalStart + left,
            Right = (double)FormatString.HorizontalEnd + left,
            Brush = brush,
            Font = font
        };

        prepared = true;

        return (h, true);
    }
    public void DrawItem(PdfDocumentObject pdo, bool newpage, double heightoffset)
    {
        if(Info is null || !prepared) return;
        try
        {
            if(newpage)
            {
                Info.Top -= heightoffset;
                Info.Bottom -= heightoffset;
            }
            XRect strrect = new(new XPoint(Info.Left, Info.Top), new XPoint(Info.Right, Info.Bottom));
            var tf = new XTextFormatter(pdo.Gfxs[pdo.CurrentIndex]);
            var format = new XStringFormat
            {
                LineAlignment = XLineAlignment.Near,
                Alignment = XStringAlignment.Near
            };
            tf.DrawString(Info.FullString, Info.Font, Info.Brush, strrect, format);
        }
        catch(Exception ex) {Logging.WriteLog("err.stringitem.draw", ex.ToString(), FormatString.PdfTemplateStringID);}
    }

    private string? GetFullString(List<PdfKeyValuePair> pairs)
    {
        if(FormatString.FormatString is null) {Logging.WriteLog("nv.formatstring.formatstring", "Value of DBObject is null.", FormatString.PdfTemplateStringID); return null;} 
        try
        {
            var sort = Keys.OrderBy(k => k.TemplateKeyValue.IndexInFormatString).ToList();
            var strings = new string[sort.Count];
            for(int i = 0; i < sort.Count; i++)
            {
                var value = FindKeyValue(pairs, sort[i].TemplateKeyValue.PdfTemplateKeyValueID);
                if(value is not null) strings[i] = value.ToString();
                else
                {
                    strings[i] = string.Empty;
                    Logging.WriteLog("mk", string.Format("Fehlender Key '{1}' in PdfFileKeyValuePairListe. PdfTemplateKeyID={0}", sort[i].TemplateKeyValue.PdfKeyID, sort[i].Key.Key), FormatString.PdfTemplateStringID);
                }
            }
            return (strings.Length > 0) ? string.Format(FormatString.FormatString, strings) : FormatString.FormatString;
        }
        catch(Exception ex){Logging.WriteLog("err.getfullstring", ex.ToString(), FormatString.PdfTemplateStringID); return null;}
    }

    private string? GetTemplateString()
    {
        if(FormatString.FormatString is null) {Logging.WriteLog("nv.formatstring.formatstring", "Value of DBObject is null.", FormatString.PdfTemplateStringID); return null;} 
        try
        {
            var sort = Keys.OrderBy(k => k.TemplateKeyValue.IndexInFormatString).ToList();
            string[] strings = new string[sort.Count];
            for(int i = 0; i < sort.Count; i++) strings[i] = string.Format("${0}$", sort[i].Key.Key);
            return (strings.Length > 0) ? string.Format(FormatString.FormatString, strings) : FormatString.FormatString;
        }
        catch(Exception ex){Logging.WriteLog("err.gettemplatestring", ex.ToString(), FormatString.PdfTemplateStringID); return null;}
    }

    private string? FindKeyValue(List<PdfKeyValuePair> pairs, int PdfTemplateKeyValueID)
    {
        foreach(var pair in pairs) if(pair.PdfTemplateKeyValueID == PdfTemplateKeyValueID) return pair.Value;
        return null;
    }
}

internal class PdfGeometryItem : ITemplateItem
{
    internal required PdfTemplateGeometry Geometry { get; set; }
    internal required List<PdfTemplatePoint> Points { get; set; } = new();

    private bool prepared = false;
    private GeometryInformation? Info { get; set; }
    private class GeometryInformation
    {
        internal required XPen Pen { get; set; }
        internal required XBrush Brush { get; set; }
        internal required PdfGeometryType Type { get; set; }
        internal required XFillMode FillMode { get; set; }
        internal required double Top { get; set; }
        internal required double Left { get; set; }
        internal required double Bottom { get; set; }
        internal required double Right { get; set; }
        internal required double StartAngle { get; set; }
        internal required double SweepAngle { get; set; }
        internal required double Tension { get; set; }
        internal required double EllipseWidth { get; set; }
        internal required double EllipseHeight { get; set; }
        internal required List<GIPoint> Points { get; set; }
    }
    private class GIPoint
    {
        internal required double X { get; set; }
        internal required double Y { get; set; }
    }

    public (double height, bool success) PrepareDraw(PdfDocumentObject pdo, PdfFileType type, PdfCustomerData data, PdfDrawInformation pdi, double top, double left)
    {
        if(Geometry.Pen is null) {Logging.WriteLog("nv.templategeometry.pen", "Value of DBObject is null.", Geometry.PdfTemplateGeometryID); return (0, false);} 
        if(Geometry.Type is null) {Logging.WriteLog("nv.templategeometry.type", "Value of DBObject is null.", Geometry.PdfTemplateGeometryID); return (0, false);} 
        if(Geometry.FillMode is null) {Logging.WriteLog("nv.templategeometry.fillmode", "Value of DBObject is null.", Geometry.PdfTemplateGeometryID); return (0, false);} 
        if(Geometry.VerticalStart is null) {Logging.WriteLog("nv.templategeometry.vstart", "Value of DBObject is null.", Geometry.PdfTemplateGeometryID); return (0, false);} 
        if(Geometry.VerticalEnd is null) {Logging.WriteLog("nv.templategeometry.vend", "Value of DBObject is null.", Geometry.PdfTemplateGeometryID); return (0, false);} 
        if(Geometry.HorizontalStart is null) {Logging.WriteLog("nv.templategeometry.hstart", "Value of DBObject is null.", Geometry.PdfTemplateGeometryID); return (0, false);} 
        if(Geometry.HorizontalEnd is null) {Logging.WriteLog("nv.templategeometry.hend", "Value of DBObject is null.", Geometry.PdfTemplateGeometryID); return (0, false);} 
        if(Geometry.StartAngle is null) {Logging.WriteLog("nv.templategeometry.startangle", "Value of DBObject is null.", Geometry.PdfTemplateGeometryID); return (0, false);} 
        if(Geometry.SweepAngle is null) {Logging.WriteLog("nv.templategeometry.sweepangle", "Value of DBObject is null.", Geometry.PdfTemplateGeometryID); return (0, false);} 
        if(Geometry.Tension is null) {Logging.WriteLog("nv.templategeometry.tension", "Value of DBObject is null.", Geometry.PdfTemplateGeometryID); return (0, false);} 
        if(Geometry.EllipseWidth is null) {Logging.WriteLog("nv.templategeometry.ewidth", "Value of DBObject is null.", Geometry.PdfTemplateGeometryID); return (0, false);} 
        if(Geometry.EllipseHeight is null) {Logging.WriteLog("nv.templategeometry.eheight", "Value of DBObject is null.", Geometry.PdfTemplateGeometryID); return (0, false);} 

        List<GIPoint> points = new();
        foreach(var point in Points)
        {
            if(point.X is null) {Logging.WriteLog("nv.templatepoint.x", "Value of DBObject is null.", point.PdfTemplatePointID); return (0, false); }
            if(point.Y is null) {Logging.WriteLog("nv.templatepoint.y", "Value of DBObject is null.", point.PdfTemplatePointID); return (0, false); }
            points.Add(new(){X = (double)point.X, Y = (double)point.Y + top});
        }

        Info = new()
        {
            Pen = new XPen(XColor.FromKnownColor((XKnownColor)(int)Geometry.Pen)),
            Brush = new XSolidBrush(XColor.FromKnownColor((XKnownColor)(int)Geometry.Pen)),
            Type = (PdfGeometryType)(int)Geometry.Type,
            FillMode = (XFillMode)(int)Geometry.FillMode,
            Top = (double)Geometry.VerticalStart + top,
            Bottom = (double)Geometry.VerticalEnd + top,
            Left = (double)Geometry.HorizontalStart + left,
            Right = (double)Geometry.HorizontalEnd + left,
            StartAngle = (double)Geometry.StartAngle,
            SweepAngle = (double)Geometry.SweepAngle,
            Tension = (double)Geometry.Tension,
            EllipseWidth = (double)Geometry.EllipseWidth,
            EllipseHeight = (double)Geometry.EllipseHeight,
            Points = points
        };

        if(!TestPointCount()) {Logging.WriteLog("pc.templategeometry", "Geometryobject doesnt have required number of points.", Geometry.PdfTemplateGeometryID); return (0, false);}

        double height = CalculateHeight();

        prepared = true;

        return (height, true);
    }

    public void DrawItem(PdfDocumentObject pdo, bool newpage, double heightoffset)
    {
        if(Info is null || !prepared) return;
        try
        {
            if(newpage)
            {
                Info.Top -= heightoffset;
                Info.Bottom -= heightoffset;
                foreach(var p in Info.Points) p.Y -= heightoffset;
            }
            DrawGeometry(pdo.Gfxs[pdo.CurrentIndex]);
        }
        catch(Exception ex) {Logging.WriteLog("err.geometryitem.draw", ex.ToString(), Geometry.PdfTemplateGeometryID);}
    }

    private void DrawGeometry(XGraphics gfx)
    {
        if(Info is null || !prepared) return;
        XPoint[] ps;
        XRect rect;
        switch(Info.Type)
        {
            case PdfGeometryType.Arc:
            rect = new(new XPoint(Info.Left, Info.Top), new XPoint(Info.Right, Info.Bottom));
            gfx.DrawArc(Info.Pen, rect, Info.StartAngle, Info.SweepAngle);
            break;
            case PdfGeometryType.Bezier:
            ps = new XPoint[4];
            for(int i = 0; i < 4; i++) ps[i] = new(Info.Points[i].X, Info.Points[i].Y);
            gfx.DrawBezier(Info.Pen, ps[0], ps[1], ps[2], ps[3]);
            break;
            case PdfGeometryType.ClosedCurve:
            ps = new XPoint[Info.Points.Count];
            for(int i = 0; i < Info.Points.Count; i++) ps[i] = new(Info.Points[i].X, Info.Points[i].Y);
            gfx.DrawClosedCurve(Info.Pen, Info.Brush, ps, Info.FillMode, Info.Tension);
            break;
            case PdfGeometryType.Curve:
            ps = new XPoint[Info.Points.Count];
            for(int i = 0; i < Info.Points.Count; i++) ps[i] = new(Info.Points[i].X, Info.Points[i].Y);
            gfx.DrawCurve(Info.Pen, ps, Info.Tension);
            break;
            case PdfGeometryType.Ellipse:
            rect = new(new XPoint(Info.Left, Info.Top), new XPoint(Info.Right, Info.Bottom));
            gfx.DrawEllipse(Info.Pen, rect);
            break;
            case PdfGeometryType.Line:
            ps = new XPoint[2];
            for(int i = 0; i < 2; i++) ps[i] = new(Info.Points[i].X, Info.Points[i].Y);
            gfx.DrawLine(Info.Pen, ps[0], ps[1]);
            break;
            case PdfGeometryType.Pie:
            rect = new(new XPoint(Info.Left, Info.Top), new XPoint(Info.Right, Info.Bottom));
            gfx.DrawPie(Info.Pen, rect, Info.StartAngle, Info.SweepAngle);
            break;
            case PdfGeometryType.Polygon:
            ps = new XPoint[Info.Points.Count];
            for(int i = 0; i < Info.Points.Count; i++) ps[i] = new(Info.Points[i].X, Info.Points[i].Y);
            gfx.DrawPolygon(Info.Brush, ps, Info.FillMode);
            break;
            case PdfGeometryType.Rectangle:
            rect = new(new XPoint(Info.Left, Info.Top), new XPoint(Info.Right, Info.Bottom));
            gfx.DrawRectangle(Info.Pen, rect);
            break;
            case PdfGeometryType.RoundedRectangle:
            rect = new(new XPoint(Info.Left, Info.Top), new XPoint(Info.Right, Info.Bottom));
            gfx.DrawRoundedRectangle(Info.Pen, rect, new XSize(Info.EllipseWidth, Info.EllipseHeight));
            break;
        }
    }

    private bool TestPointCount()
    {
        if(Info is null) return false;
        switch(Info.Type)
        {
            case PdfGeometryType.Bezier:
            if(Info.Points.Count != 4) return false; break;
            case PdfGeometryType.ClosedCurve:
            if(Info.Points.Count < 2) return false; break;
            case PdfGeometryType.Curve:
            if(Info.Points.Count < 2) return false; break;
            case PdfGeometryType.Line:
            if(Info.Points.Count != 2) return false; break;
            case PdfGeometryType.Polygon:
            if(Info.Points.Count < 3) return false; break;
            default: return true;
        }
        return true;
    }

    private double CalculateHeight()
    {
        if(Info is null) return 0;
        double max = 0;
        switch(Info.Type)
        {
            case PdfGeometryType.Arc:
            return Info.Bottom;
            case PdfGeometryType.Bezier:
            foreach(var p in Info.Points) if(p.Y > max) max = p.Y;
            return max;
            case PdfGeometryType.ClosedCurve:
            foreach(var p in Info.Points) if(p.Y > max) max = p.Y;
            return max;
            case PdfGeometryType.Curve:
            foreach(var p in Info.Points) if(p.Y > max) max = p.Y;
            return max;
            case PdfGeometryType.Ellipse:
            return Info.Bottom;
            case PdfGeometryType.Line:
            foreach(var p in Info.Points) if(p.Y > max) max = p.Y;
            return max;
            case PdfGeometryType.Pie:
            return Info.Bottom;
            case PdfGeometryType.Polygon:
            foreach(var p in Info.Points) if(p.Y > max) max = p.Y;
            return max;
            case PdfGeometryType.Rectangle:
            return Info.Bottom;
            case PdfGeometryType.RoundedRectangle:
            return Info.Bottom;
        }
        return 0;
    }
}

internal class PdfImageItem : ITemplateItem
{
    internal required PdfTemplateImage Image { get; set; }
    internal required Data Data { get; set; }

    private bool prepared = false;
    private ImageInformation? Info { get; set; }
    private class ImageInformation
    {
        public required double Left { get; set; }
        public required double Top { get; set;}
        public required double Right { get; set;}
        public required double Bottom { get; set;}
        public required byte[] Bytes { get; set; }
    }
    
    public (double height, bool success) PrepareDraw(PdfDocumentObject pdo, PdfFileType type, PdfCustomerData data, PdfDrawInformation pdi, double top, double left)
    {
        if(Image.VerticalStart is null) {Logging.WriteLog("nv.templateimage.verticalstart", "Value of DBObject is null.", Image.PdfTemplateImageID); return (0, false);} 
        if(Image.VerticalEnd is null) {Logging.WriteLog("nv.templateimage.verticalend", "Value of DBObject is null.", Image.PdfTemplateImageID); return (0, false);} 
        if(Image.HorizontalStart is null) {Logging.WriteLog("nv.templateimage.horizontalstart", "Value of DBObject is null.", Image.PdfTemplateImageID); return (0, false);}
        if(Image.HorizontalEnd is null) {Logging.WriteLog("nv.templateimage.horizontalend", "Value of DBObject is null.", Image.PdfTemplateImageID); return (0, false);} 
        if(Data.Bytes is null) {Logging.WriteLog("nv.data.bytes", "Value of DBObject is null.", Data.DataID); return (0, false);} 

        double height = (double)Image.VerticalEnd;

        Info = new()
        {
            Left = (double)Image.HorizontalStart + left,
            Right = (double)Image.HorizontalEnd + left,
            Top = (double)Image.VerticalStart + top,
            Bottom = (double)Image.VerticalEnd + top,
            Bytes = Data.Bytes
        };

        prepared = true;

        return (height, true);
    }

    public void DrawItem(PdfDocumentObject pdo, bool newpage, double heightoffset)
    {
        if(Info is null || !prepared) return;
        try{
            if(newpage)
            {
                Info.Top -= heightoffset;
                Info.Bottom -= heightoffset;
            }
            XRect imgrect = new(new XPoint(Info.Left, Info.Top), new XPoint(Info.Right, Info.Bottom));
            using(MemoryStream stream = new(Info.Bytes, 0, Info.Bytes.Length, true, true))
            {
                XImage img = XImage.FromStream(stream);
                pdo.Gfxs[pdo.CurrentIndex].DrawImage(img, imgrect);
            }
        }
        catch(Exception ex) {Logging.WriteLog("err.imageitem.draw", ex.ToString(), Image.PdfTemplateImageID);}
    }
}