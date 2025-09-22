using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using X.Neurons.P.LeoLedtech.Test.Server.Models.PDF;

namespace X.Neurons.P.LeoLedtech.Test.Server.BasicUtils
{
    public class TestReportDocument : IDocument
    {
        private readonly TestReport _report;

        public TestReportDocument(TestReport report)
        {
            _report = report;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily(Fonts.Arial));

                    page.Header()
             .AlignCenter()
             .Text("測試報告")
             .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            // 報告基本信息
                            column.Item().Element(ComposeHeader);

                            // 間距
                            column.Item().PaddingTop(20);

                            // 詳細測試記錄
                            column.Item().Element(ComposeTestDetails);
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("第 ");
                            x.CurrentPageNumber();
                            x.Span(" 頁，共 ");
                            x.TotalPages();
                            x.Span(" 頁");
                        });
                });
        }

        void ComposeHeader(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text($"工單號: {_report.WorkOrderNumber}").FontSize(14).SemiBold();
                        col.Item().PaddingTop(5).Text($"產品序號: {_report.ProductSerialNumber}").FontSize(14).SemiBold();
                        col.Item().PaddingTop(5).Text($"測試日期時間: {_report.TestDateTime}").FontSize(14).SemiBold();
                    });
                });
            });
        }

        void ComposeTestDetails(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Text("詳細紀錄").FontSize(16).SemiBold().FontColor(Colors.Blue.Medium);
                column.Item().PaddingTop(10);

                foreach (var step in _report.TestSteps)
                {
                    column.Item().Element(c => ComposeTestStep(c, step));
                    column.Item().PaddingTop(15);
                }
            });
        }

        void ComposeTestStep(IContainer container, TestStep step)
        {
            container.Column(column =>
            {
                // 測試步驟標題
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        //col.Item().Text($"步驟: {step.StepNumber}").FontSize(12).SemiBold();
                        col.Item().Text($"測試時間: {step.TestTime}").FontSize(11);
                        col.Item().Text($"步驟詳情: {step.StepDetails}").FontSize(11);
                        if (step.ChannelResults.Any(r => r.Status == false))
                        {
                            col.Item().Text($"狀態: 未通過").FontSize(11).FontColor(Colors.Red.Medium);
                        }
                        else
                        {
                            col.Item().Text($"狀態: 通過").FontSize(11).FontColor(Colors.Green.Medium);
                        }
                            //col.Item().Text($"狀態: {step.ChannelResults.Find(r => r.Status == false)}").FontSize(11).FontColor(Colors.Green.Medium);
                    });
                });

                // 通道測試結果表格
                column.Item().PaddingTop(10).Element(c => ComposeChannelResultsTable(c, step.ChannelResults));
            });
        }

        void ComposeChannelResultsTable(IContainer container, List<ChannelResult> results)
        {
            container.Table(table =>
            {
                // 定義列寬
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                });

                // 表格標題
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).AlignCenter().Text("通道").SemiBold();
                    header.Cell().Element(CellStyle).AlignCenter().Text("電壓").SemiBold();
                    header.Cell().Element(CellStyle).AlignCenter().Text("電流").SemiBold();
                    header.Cell().Element(CellStyle).AlignCenter().Text("功率").SemiBold();

                    static IContainer CellStyle(IContainer container)
                    {
                        return container
                            .DefaultTextStyle(x => x.FontSize(11))
                            .PaddingVertical(5)
                            .PaddingHorizontal(8)
                            .BorderBottom(1)
                            .BorderColor(Colors.Black);
                    }
                });

                // 表格內容
                foreach (var result in results)
                {
                    table.Cell().Element(CellStyle).AlignCenter().Text(result.Channel);
                    table.Cell().Element(CellStyle).AlignCenter().Text(result.Voltage);
                    table.Cell().Element(CellStyle).AlignCenter().Text(result.Current);
                    table.Cell().Element(CellStyle).AlignCenter().Text(result.Power);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container
                            .DefaultTextStyle(x => x.FontSize(10))
                            .PaddingVertical(3)
                            .PaddingHorizontal(8)
                            .BorderBottom(0.5f)
                            .BorderColor(Colors.Grey.Lighten2);
                    }
                }
            });
        }
    }
}
