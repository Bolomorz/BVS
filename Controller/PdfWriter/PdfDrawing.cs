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
    internal required List<PdfTemplateItemCollection> ItemCollections { get; set; }
    internal PdfDocument? Draw(PdfFileType type, List<PdfData> data)
    {
        if(Template.Name is null) {Logging.WriteLog("nv.template.name", "Value of DBObject is null."); return null;}
        if(Template.VerticalStart is null) {Logging.WriteLog("nv.template.verticalstart", "Value of DBObject is null."); return null;}
        if(Template.VerticalEnd is null) {Logging.WriteLog("nv.template.verticalend", "Value of DBObject is null."); return null;}
        if(Template.HorizontalStart is null) {Logging.WriteLog("nv.template.horizontalstart", "Value of DBObject is null."); return null;}
        if(Template.HorizontalEnd is null) {Logging.WriteLog("nv.template.horizontalend", "Value of DBObject is null."); return null;}
        if(Template.Orientation is null) {Logging.WriteLog("nv.template.orientation", "Value of DBObject is null."); return null;}
        
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

        foreach(var ic in ItemCollections)
        {
            if(pdi.PreviousSuccess) ic.Draw(pdo, type, data, pdi);
        }

        return pdi.PreviousSuccess ? pdo.Document : null;
    }
}

internal class PdfTemplateItemCollection
{
    internal required PdfTemplateItem Item { get; set; }
    internal required List<ITemplateItem> Items { get; set; }

    internal void Draw(PdfDocumentObject pdo, PdfFileType type, List<PdfData> data, PdfDrawInformation pdi)
    {
        if(Item.VerticalOffsetToPreviousItem is null) {Logging.WriteLog("nv.templateitem.offset", "Value of DBObject is null."); pdi.PreviousSuccess = false; return;}
        if(Item.AlwaysStartNewPage is null) {Logging.WriteLog("nv.templateitem.newpage", "Value of DBObject is null."); pdi.PreviousSuccess = false; return;}
        if(Item.RepeatForEachDataObject is null) {Logging.WriteLog("nv.templateitem.repeat", "Value of DBObject is null."); pdi.PreviousSuccess = false; return;}
        
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
            foreach(var d in data)
            {
                double need = 0;
                foreach(var item in Items)
                {
                    double h = item.MeasureNeededHeight(pdo, type, d, pdi, newpos);
                    if(h > need) need = h;
                }
                if(newpos + need > pdi.VerticalEnd) 
                {
                    pdo.AddPage();
                    newpos = pdi.VerticalStart;
                }
                foreach(var item in Items)
                {
                    bool success = item.Draw(pdo, type, d, newpos);
                    if(!success) {pdi.PreviousSuccess = false; return;}
                }
            }
        }
        else
        {
            var d = data[0];
            double need = 0;
            foreach(var item in Items)
            {
                double h = item.MeasureNeededHeight(pdo, type, d, pdi, newpos);
                if(h > need) need = h;
            }
            if(newpos + need > pdi.VerticalEnd) 
            {
                pdo.AddPage();
                newpos = pdi.VerticalStart;
            }
            foreach(var item in Items)
            {
                bool success = item.Draw(pdo, type, d, newpos);
                if(!success) {pdi.PreviousSuccess = false; return;}
            }
        }
    }
}

internal interface ITemplateItem
{
    double MeasureNeededHeight(PdfDocumentObject pdo, PdfFileType type, PdfData data, PdfDrawInformation pdi, double top);
    bool Draw(PdfDocumentObject pdo, PdfFileType type, PdfData data, double top);
}