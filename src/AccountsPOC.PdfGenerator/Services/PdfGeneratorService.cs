using AccountsPOC.Domain.Entities;
using AccountsPOC.PdfGenerator.Models;
using AccountsPOC.PdfGenerator.Templates;
using QuestPDF.Fluent;

namespace AccountsPOC.PdfGenerator.Services;

public interface IPdfGeneratorService
{
    byte[] GenerateSalesOrderPdf(SalesOrder salesOrder, TenantBrandingInfo branding);
    byte[] GenerateSalesInvoicePdf(SalesInvoice salesInvoice, TenantBrandingInfo branding);
    byte[] GeneratePartialDispatchPdf(PartialDispatch dispatch, TenantBrandingInfo branding);
}

public class PdfGeneratorService : IPdfGeneratorService
{
    static PdfGeneratorService()
    {
        // Set QuestPDF license (Community license for non-commercial use)
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
    }

    public byte[] GenerateSalesOrderPdf(SalesOrder salesOrder, TenantBrandingInfo branding)
    {
        var document = new SalesOrderPdfTemplate(salesOrder, branding);
        return document.GeneratePdf();
    }

    public byte[] GenerateSalesInvoicePdf(SalesInvoice salesInvoice, TenantBrandingInfo branding)
    {
        var document = new SalesInvoicePdfTemplate(salesInvoice, branding);
        return document.GeneratePdf();
    }

    public byte[] GeneratePartialDispatchPdf(PartialDispatch dispatch, TenantBrandingInfo branding)
    {
        var document = new PartialDispatchPdfTemplate(dispatch, branding);
        return document.GeneratePdf();
    }
}
