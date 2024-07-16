using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace BVS;

public static class PdfWriterLogic
{
    internal static PdfFile WritePdfWithData(int pdfTemplateID, List<int> customerIDs, List<KeyValue> KeyValues)
    {
        try
        {
            var template = DBFunctions.GetPdfTemplateObject(pdfTemplateID);
            if(template is null) {Logging.WriteLog("nf.template", "PdfTemplate with this ID could not be found", pdfTemplateID); return new(){Success = false};}
            List<CustomerObject> customers = new();
            foreach(var id in customerIDs)
            {
                var customer = DBFunctions.GetCustomerObject(id);
                if(customer is null) {Logging.WriteLog("nf.customer","Customer with this ID could not be found", id); return new(){Success = false};}
                customers.Add(customer);
            }

            List<PdfCustomerData> pdfCustomerDatas = new();
            foreach(var customer in customers)
            {
                List<PdfKeyValuePair> pairs = new();
                foreach(var dynamic in template.DynamicItemCollections)
                {
                    foreach(var item in dynamic.Items)
                    {
                        if(item is PdfStringItem)
                        {
                            var stringitem = (PdfStringItem)item;
                            foreach(var key in stringitem.Keys)
                            {
                                if(key.Key.Key is null) continue;
                                var value = GetKeyValue(KeyValues, customer, template, key.Key.Key);
                                if(value is not null) pairs.Add(new(){Value = value, PdfTemplateKeyValueID = key.TemplateKeyValue.PdfTemplateKeyValueID});
                                else {Logging.WriteLog("nf.keynotfound", string.Format("Key {0} could not be found.", key.Key.Key), key.Key.PdfKeyID); return new(){Success = false};}
                            }
                        }
                    }
                }
                pdfCustomerDatas.Add(new(){Data = pairs});
            }

            PdfData data = new(){PdfTemplateID = pdfTemplateID, CustomerIDs = customerIDs, PdfCustomerDatas = pdfCustomerDatas};

            var document = template.Draw(PdfFileType.Document, data);
            if(document is null) {Logging.WriteLog("nv.document", "template.draw returns null", template.Template.PdfTemplateID); return new(){Success = false};}
            var success = TransformAndSaveFile(document, template, customers, PdfFileType.Document);
            return success;
        }
        catch (Exception ex)
        {
            Logging.WriteLog("pwl.writepdfwithdata", ex.ToString(), 0);
            return new(){Success = false};
        }
    }

    internal static PdfFile WritePdfTemplateFile(int pdfTemplateID)
    {
        try
        {
            var template = DBFunctions.GetPdfTemplateObject(pdfTemplateID);
            if(template is null) {Logging.WriteLog("nf.template", "PdfTemplate with this ID could not be found", pdfTemplateID); return new(){Success = false};}

            PdfData data = new()
            {
                PdfTemplateID = pdfTemplateID,
                CustomerIDs = new List<int>(),
                PdfCustomerDatas = new List<PdfCustomerData>(){new PdfCustomerData()}
            };

            var document = template.Draw(PdfFileType.Template, data);
            if(document is null) {Logging.WriteLog("nv.document", "template.draw returns null", template.Template.PdfTemplateID); return new(){Success = false};}
            var success = TransformAndSaveFile(document, template, new List<CustomerObject>(), PdfFileType.Template);
            return success;
        }
        catch (Exception ex)
        {
            Logging.WriteLog("pwl.writepdftemplatefile", ex.ToString(), 0);
            return new(){Success = false};
        }
    }

    public static void OpenDocument(byte[] file, string filepath)
    {
        PdfDocument document;
        using (var stream = new MemoryStream(file))
        {
            document = PdfReader.Open(stream);
            document.Save(filepath);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filepath){UseShellExecute = true});
        }
    }

    private static string? GetKeyValue(List<KeyValue> pairs, CustomerObject co, PdfTemplateObject to, string key)
    {
        var value = GetKeyValueFromList(pairs, key);
        if(value is null) value = co.GetKeyValue(key);
        int i = 0;
        while(value is null && i < co.CustomerInvoiceItemObjects.Count)
        {
            value = co.CustomerInvoiceItemObjects[i].GetKeyValue(key);
        }
        if(value is null) value = to.GetKeyValueFromTemplate(key);
        return value;
    }

    private static string? GetKeyValueFromList(List<KeyValue> pairs, string key)
    {
        foreach(var pair in pairs) if(key == pair.Key) return pair.Value;
        return null;
    }

    private static PdfFile TransformAndSaveFile(PdfDocument document, PdfTemplateObject pdo, List<CustomerObject> cos, PdfFileType type)
    {
        try
        {
            byte[]? pdf = null;
            using(MemoryStream stream = new())
            {
                document.Save(stream, false);
                pdf = stream.ToArray();
            }
            if(pdf is null) {Logging.WriteLog("nv.pdfbytes", "Error while trying to create bytearray from pdf document", pdo.Template.PdfTemplateID); return new(){Success = false};}

            string filename;
            switch(type)
            {
                case PdfFileType.Document:
                break;
                case PdfFileType.Template:
                break;
            }
        }
    }
}