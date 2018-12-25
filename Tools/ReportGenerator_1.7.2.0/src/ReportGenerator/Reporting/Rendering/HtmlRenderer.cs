using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using log4net;
using Palmmedia.ReportGenerator.Parser.Analysis;
using Palmmedia.ReportGenerator.Properties;

namespace Palmmedia.ReportGenerator.Reporting.Rendering
{
    /// <summary>
    /// HTML report renderer.
    /// </summary>
    internal class HtmlRenderer : RendererBase, IReportRenderer, IDisposable
    {
        #region HTML Snippets

        /// <summary>
        /// The head of each generated HTML file.
        /// </summary>
        private const string HtmlStart = @"<!DOCTYPE html>
<html>
<html><head>
<meta charset=""utf-8"" />
<title>{0} - {1}</title>
<link rel=""stylesheet"" type=""text/css"" href=""report.css"" />
{2}
</head><body><div class=""container"">";

        /// <summary>
        /// The java script for the summary report.
        /// </summary>
        private const string SummaryJavaScript = @"<script type=""text/javascript"">
/* <![CDATA[ */
document.getElementsByClassName = function(cl) {
  var retnode = [];
  var myclass = new RegExp('\\b'+cl+'\\b');
  var elem = this.getElementsByTagName('*');
  for (var i = 0; i < elem.length; i++) {
    var classes = elem[i].className;
    if (myclass.test(classes)) {
      retnode.push(elem[i]);
    }
  }
  return retnode;
};

function getSiblingNode(element) {
    do {
        element = element.nextSibling;
    } while (element && element.nodeType != 1);
    return element;
}

function toggleAssemblyDetails() {
  var popup = getSiblingNode(this); 
  if (popup.style.display == 'block') { 
    popup.style.display = 'none'; 
  }
  else {
    var popups = document.getElementsByClassName('detailspopup');
    for (var i = 0, j = popups.length; i < j; i++) { 
      popups[i].style.display = 'none';
    }
    popup.style.display = 'block'; 
  }
  return false;
}

function collapseAllClasses() {
  var classRows = document.getElementsByClassName('classrow');
  for (var i = 0, j = classRows.length; i < j; i++) {
    classRows[i].style.display = 'none';
  }
  var expandedRows = document.getElementsByClassName('expanded');
  for (var i = 0, j = expandedRows.length; i < j; i++) {
    expandedRows[i].className = 'collapsed';
  }
  return false;
}

function expandAllClasses() {
  var classRows = document.getElementsByClassName('classrow');
  for (var i = 0, j = classRows.length; i < j; i++) {
    classRows[i].style.display = '';
  }
  var collapsedRows = document.getElementsByClassName('collapsed');
  for (var i = 0, j = collapsedRows.length; i < j; i++) {
    collapsedRows[i].className = 'expanded';
  }
  return false;
}

function toggleClassesInAssembly() {
  var assemblyRow = this.parentNode.parentNode;
  assemblyRow.className = assemblyRow.className == 'collapsed' ? 'expanded' : 'collapsed';
  var classRow = getSiblingNode(assemblyRow);
  while (classRow && classRow.className == 'classrow') {
    classRow.style.display = classRow.style.display == 'none' ? '' : 'none';
    classRow = getSiblingNode(classRow);
  }
  return false;
}

function init() {
  var toggleAssemblyDetailsLinks = document.getElementsByClassName('toggleAssemblyDetails');
  for (var i = 0, j = toggleAssemblyDetailsLinks.length; i < j; i++) {
    toggleAssemblyDetailsLinks[i].onclick = toggleAssemblyDetails;
  }

  document.getElementById('collapseAllClasses').onclick = collapseAllClasses;
  document.getElementById('expandAllClasses').onclick = expandAllClasses;

  var toggleClassesInAssemblyLinks = document.getElementsByClassName('toggleClassesInAssembly');
  for (var i = 0, j = toggleClassesInAssemblyLinks.length; i < j; i++) {
    toggleClassesInAssemblyLinks[i].onclick = toggleClassesInAssembly;
  }
}

window.onload = init;
/* ]]> */
</script>";

        /// <summary>
        /// The java script for the class report.
        /// </summary>
        private const string ClassJavaScript = @"<script type=""text/javascript"">
/* <![CDATA[ */
document.getElementsByClassName = function(cl) {
  var retnode = [];
  var myclass = new RegExp('\\b'+cl+'\\b');
  var elem = this.getElementsByTagName('*');
  for (var i = 0; i < elem.length; i++) {
    var classes = elem[i].className;
    if (myclass.test(classes)) {
      retnode.push(elem[i]);
    }
  }
  return retnode;
};

function switchTestMethod() {
  var testMethodName = this.getAttribute('value');

  var lineCoverageTables = document.getElementsByClassName('lineAnalysis');

  for (var i = 0, j = lineCoverageTables.length; i < j; i++) {
    var lines = lineCoverageTables[i].getElementsByTagName('tr');
    for (var k = 1, l = lines.length; k < l; k++) {
      var coverageData = JSON.parse(lines[k].getAttribute('data-coverage').replace(/'/g, '""'));
      var lineAnalysis = coverageData[testMethodName];
      var cells = lines[k].getElementsByTagName('td');
      if (lineAnalysis == null) {
        lineAnalysis = coverageData['AllTestMethods'];
        if (lineAnalysis.LVS != 'gray') {
          cells[0].setAttribute('class', 'red');
          cells[1].innerText = '0';
        }
      } else {
        cells[0].setAttribute('class', lineAnalysis.LVS);
        cells[1].innerText = lineAnalysis.VC;
      }
    }
  }
}

function togglePin() {
  var testMethodElement = document.getElementById('testmethods');
  testMethodElement.className = testMethodElement.className == '' ? 'pinned' : '';

  return false;
}

function init() {
  var testMethodInputElements = document.getElementsByTagName('input');
  for (var i = 0, j = testMethodInputElements.length; i < j; i++) {
    testMethodInputElements[i].onchange = switchTestMethod;
  }

  document.getElementById('pin').onclick = togglePin;

  if (navigator.appName == 'Microsoft Internet Explorer') {
    document.getElementById('pinheader').style.width = 'auto';
  }
}

window.onload = init;
/* ]]> */
</script>";

        /// <summary>
        /// The default css.
        /// </summary>
        private const string DefaultCss = @"html { font-family: sans-serif; margin: 20px; font-size: 0.9em; background-color: #f5f5f5; }
h1 { font-size: 1.2em; font-weight: bold; margin: 20px 0 15px 0; padding: 0; }
h2 { font-size: 1.0em; font-weight: bold; margin: 10px 0 15px 0; padding: 0; }
th { text-align: left; }
a { color: #cc0000; text-decoration: none; }
a:hover { color: #000000; text-decoration: none; }
.container { margin: auto; max-width: 1200px; width: 90%; border: solid 1px #a7bac5; padding: 0 20px 20px 20px; background-color: #ffffff; }
.overview { border: solid 1px #a7bac5; border-collapse: collapse; width: 100%; word-wrap: break-word; table-layout:fixed; }
.overview th { border: solid 1px #a7bac5; border-collapse: collapse; padding: 2px 5px 2px 5px; background-color: #d2dbe1; }
.overview td { border: solid 1px #a7bac5; border-collapse: collapse; padding: 2px 5px 2px 5px; }
.coverage { border: solid 1px #a7bac5; border-collapse: collapse; font-size: 5px; }
.coverage td { padding: 0; }
.toggleClasses { font-size: 0.7em; padding: 0 0 0 5px; margin: 0 0 3px 0; }
tr.expanded a.toggleClassesInAssembly { width: 12px; height: 12px; display: inline-block; background-image: url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAwAAAAMCAYAAABWdVznAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAadEVYdFNvZnR3YXJlAFBhaW50Lk5FVCB2My41LjEwMPRyoQAAAClJREFUKFNj+P//PwMpmCTFIIMHo4YzQFeRghlIUQxSOxg9TUoskxVxAAc+kbB1wVv5AAAAAElFTkSuQmCC); }
tr.collapsed a.toggleClassesInAssembly { width: 12px; height: 12px; display: inline-block; background-image: url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAwAAAAMCAYAAABWdVznAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAadEVYdFNvZnR3YXJlAFBhaW50Lk5FVCB2My41LjEwMPRyoQAAACdJREFUKFNj+P//PwM6ZmBg+A/CWOWGgwaYB0mgISFCNB4OoUSqHwDx71a4nIsouAAAAABJRU5ErkJggg==); }
.toggleAssemblyDetails { float:right; font-size: 0.7em; margin-top: 3px; }
.detailspopup { border: solid 1px #a7bac5; width: 250px; position: absolute; background-color: #ffffff; margin: 2px 0 0 517px; padding: 10px; display: none; font-weight: normal; }
#testmethods { position: fixed; right: 0; top: 200px; background-color: #ffffff; width: 0; height: 300px; -webkit-transition:all .5s linear;-moz-transition:all .5s linear;transition:all .5s; border-left: solid 25px #a7bac5; border-top: solid 1px #a7bac5; border-bottom: solid 1px #a7bac5; }
#testmethods:hover, #testmethods.pinned { width: 600px; }
#testmethods h2 { writing-mode: bt-rl; -moz-transform: rotate(270deg); -o-transform: rotate(270deg); -webkit-transform: rotate(270deg); text-align: center; width: 300px; height: 300px; padding: 0; margin: 0; position: absolute; left: -21px; }
#testmethods div { position: absolute; left: 10px; top: 10px; overflow: auto; height: 280px; font-size: 0.9em; }
#testmethods span { max-width: 580px; white-space: nowrap; display: block; }
#pin { position: absolute; left: -25px; width: 25px; height: 300px; background-position: 4px 2px; background-repeat: no-repeat; background-image: url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABMAAAAPCAYAAAAGRPQsAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAadEVYdFNvZnR3YXJlAFBhaW50Lk5FVCB2My41LjEwMPRyoQAAAOVJREFUOE+dkzsShCAMhr0TR7GxsKW1sPUAVngCWofK2hn6PVeWwPBMmGW2+Bwh5CcvJgBIGHuDFAIEslyg7cdtZ/sv0o8+1yBSMJ+vM1GnHv5j9E6EPNvtzLwjR50ag9TjqZL0pGbEiwjNc8HRuWA6luyENeJqx7ODaho0+SjczcYZxoUCbYOqyDxuJNSgKBHjOolF73a4gIiVCySmigex2HMjkFnheJqalYtIEsFa9iJkXkglUqI2KoDpp/3/xUJKtNv1eBCRSHAMIjg2SdyPUT3Y8ZWwQoixL9RjQ4fUn0v1veELzKqo/wKmCB4AAAAASUVORK5CYII=);}
#pin:hover { cursor: pointer; background-position: 4px 4px; }
#testmethods.pinned #pin { background-image: url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABMAAAAPCAYAAAAGRPQsAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAadEVYdFNvZnR3YXJlAFBhaW50Lk5FVCB2My41LjEwMPRyoQAAAOVJREFUOE+dkzsShCAMhrmZhTex8gx2HsHSE2CtHVYWFM7snbIEBkxImHW2+Bwh5CcvDAAUrFvBGwMfpJvhcD5sP/ZflJ9j6pMI4Z72YJJOLeLHLqMQigxrMOuOGjw1Bb+8T1Wk5xdFnERotxmuxgXm6h4nrJFWO50RzqpBJkYRbrbB8F4oUTeIRRYJI3G+FBViWiex6M0OE4QYXSA5VTyIxb6JM6eHa6tqRheZIoK1bEWovBAmQjkHKYDpl/3/xVJKstt8PIRIJjkmERybIh7HiA92fiWqEGLdDnxs5JDGc6W+K3wBefLG3rZ60jAAAAAASUVORK5CYII=);}
.right { text-align: right; padding-right: 8px; }
.light { color: #888888; }
.leftmargin { padding-left: 5px; }
.green { background-color: #00ff21; }
.red { background-color: #ff0000; }
.gray { background-color: #dcdcdc; }
.footer { font-size: 0.7em; text-align: center; margin-top: 35px; }";

        /// <summary>
        /// The end of each generated HTML file.
        /// </summary>
        private const string HtmlEnd = "</div></body></html>";

        #endregion

        /// <summary>
        /// The Logger.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(HtmlRenderer));

        /// <summary>
        /// Dictionary containing the filenames of the class reports by class.
        /// </summary>
        private static readonly Dictionary<string, string> FileNameByClass = new Dictionary<string, string>();

        /// <summary>
        /// The report builder.
        /// </summary>
        private TextWriter reportTextWriter;

        /// <summary>
        /// Begins the summary report.
        /// </summary>
        /// <param name="targetDirectory">The target directory.</param>
        /// <param name="title">The title.</param>
        public void BeginSummaryReport(string targetDirectory, string title)
        {
            string targetPath = Path.Combine(targetDirectory, "index.htm");
            this.CreateTextWriter(targetPath);

            this.reportTextWriter.WriteLine(HtmlStart, WebUtility.HtmlEncode(title), Resources.CoverageReport, SummaryJavaScript);
        }

        /// <summary>
        /// Begins the class report.
        /// </summary>
        /// <param name="targetDirectory">The target directory.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="className">Name of the class.</param>
        public void BeginClassReport(string targetDirectory, string assemblyName, string className)
        {
            string fileName = GetClassReportFilename(assemblyName, className);

            this.CreateTextWriter(Path.Combine(targetDirectory, fileName));

            this.reportTextWriter.WriteLine(HtmlStart, WebUtility.HtmlEncode(className), Resources.CoverageReport, ClassJavaScript);
        }

        /// <summary>
        /// Adds a header to the report.
        /// </summary>
        /// <param name="text">The text.</param>
        public void Header(string text)
        {
            this.reportTextWriter.WriteLine("<h1>{0}</h1>", WebUtility.HtmlEncode(text));
        }

        /// <summary>
        /// Adds the test methods to the report.
        /// </summary>
        /// <param name="testMethods">The test methods.</param>
        public void TestMethods(IEnumerable<TestMethod> testMethods)
        {
            if (!testMethods.Any())
            {
                return;
            }

            this.reportTextWriter.WriteLine("<div id=\"testmethods\">");
            this.reportTextWriter.WriteLine("<h2 id=\"pinheader\">{0}</h2>", Resources.Testmethods);
            this.reportTextWriter.WriteLine("<a id=\"pin\">&nbsp;</a>");
            this.reportTextWriter.WriteLine("<div>");

            int counter = 0;

            this.reportTextWriter.WriteLine("<span><input type=\"radio\" name= \"method\" value=\"AllTestMethods\" id=\"method{1}\" checked=\"checked\" /><label for=\"method{1}\" title=\"{0}\">{0}</label></span>", Resources.All, counter);

            foreach (var testMethod in testMethods)
            {
                counter++;
                this.reportTextWriter.WriteLine(
                    "<span><input type=\"radio\" name= \"method\" value=\"M{0}\" id=\"method{0}\" /><label for=\"method{0}\" title=\"{2}\">{1}</label></span>",
                    testMethod.Id,
                    WebUtility.HtmlEncode(testMethod.ShortName),
                    WebUtility.HtmlEncode(testMethod.Name));
            }

            this.reportTextWriter.WriteLine("</div>");
            this.reportTextWriter.WriteLine("</div>");
        }

        /// <summary>
        /// Adds a file of a class to a report.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        public void File(string path)
        {
            this.reportTextWriter.WriteLine("<h2 id=\"{0}\">{1}</h2>", WebUtility.HtmlEncode(HtmlRenderer.ReplaceInvalidXmlChars(path)), WebUtility.HtmlEncode(path));
        }

        /// <summary>
        /// Adds a paragraph to the report.
        /// </summary>
        /// <param name="text">The text.</param>
        public void Paragraph(string text)
        {
            this.reportTextWriter.WriteLine("<p>{0}</p>", WebUtility.HtmlEncode(text));
        }

        /// <summary>
        /// Adds a table with two columns to the report.
        /// </summary>
        public void BeginKeyValueTable()
        {
            this.reportTextWriter.WriteLine("<table class=\"overview\">");
            this.reportTextWriter.WriteLine("<colgroup>");
            this.reportTextWriter.WriteLine("<col width=\"130\" />");
            this.reportTextWriter.WriteLine("<col />");
            this.reportTextWriter.WriteLine("</colgroup>");
        }

        /// <summary>
        /// Adds a summary table to the report.
        /// </summary>
        public void BeginSummaryTable()
        {
            this.reportTextWriter.WriteLine("<p class=\"toggleClasses\"><a id=\"collapseAllClasses\" href=\"#\">" + Resources.CollapseAllAssemblies + "</a> | <a id=\"expandAllClasses\" href=\"#\">" + Resources.ExpandAllAssemblies + "</a></p>");

            this.reportTextWriter.WriteLine("<table class=\"overview\">");
            this.reportTextWriter.WriteLine("<colgroup>");
            this.reportTextWriter.WriteLine("<col />");
            this.reportTextWriter.WriteLine("<col width=\"60\" />");
            this.reportTextWriter.WriteLine("<col width=\"105\" />");
            this.reportTextWriter.WriteLine("</colgroup>");
        }

        /// <summary>
        /// Adds a metrics table to the report.
        /// </summary>
        /// <param name="headers">The headers.</param>
        public void BeginMetricsTable(IEnumerable<string> headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }

            this.reportTextWriter.WriteLine("<table class=\"overview\">");
            this.reportTextWriter.Write("<tr>");

            foreach (var header in headers)
            {
                this.reportTextWriter.Write("<th>{0}</th>", WebUtility.HtmlEncode(header));
            }

            this.reportTextWriter.WriteLine("</tr>");
        }

        /// <summary>
        /// Adds a file analysis table to the report.
        /// </summary>
        /// <param name="headers">The headers.</param>
        public void BeginLineAnalysisTable(IEnumerable<string> headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }

            this.reportTextWriter.WriteLine("<table class=\"lineAnalysis\">");
            this.reportTextWriter.Write("<tr>");

            foreach (var header in headers)
            {
                this.reportTextWriter.Write("<th>{0}</th>", WebUtility.HtmlEncode(header));
            }

            this.reportTextWriter.WriteLine("</tr>");
        }

        /// <summary>
        /// Adds a table row with two cells to the report.
        /// </summary>
        /// <param name="key">The text of the first column.</param>
        /// <param name="value">The text of the second column.</param>
        public void KeyValueRow(string key, string value)
        {
            this.reportTextWriter.WriteLine(
                "<tr><th>{0}</th><td>{1}</td></tr>",
                WebUtility.HtmlEncode(key),
                WebUtility.HtmlEncode(value));
        }

        /// <summary>
        /// Adds a table row with two cells to the report.
        /// </summary>
        /// <param name="key">The text of the first column.</param>
        /// <param name="files">The files.</param>
        public void KeyValueRow(string key, IEnumerable<string> files)
        {
            string value = string.Join("<br />", files.Select(v => string.Format(CultureInfo.InvariantCulture, "<a href=\"#{0}\">{1}</a>", WebUtility.HtmlEncode(ReplaceInvalidXmlChars(v)), WebUtility.HtmlEncode(v))));

            this.reportTextWriter.WriteLine(
                "<tr><th>{0}</th><td>{1}</td></tr>",
                WebUtility.HtmlEncode(key),
                value);
        }

        /// <summary>
        /// Adds the given metric values to the report.
        /// </summary>
        /// <param name="metric">The metric.</param>
        public void MetricsRow(MethodMetric metric)
        {
            if (metric == null)
            {
                throw new ArgumentNullException("metric");
            }

            this.reportTextWriter.Write("<tr>");

            this.reportTextWriter.Write("<td title=\"{0}\">{1}</td>", WebUtility.HtmlEncode(metric.Name), WebUtility.HtmlEncode(metric.ShortName));

            foreach (var metricValue in metric.Metrics.Select(m => m.Value))
            {
                this.reportTextWriter.Write("<td>{0}</td>", metricValue);
            }

            this.reportTextWriter.WriteLine("</tr>");
        }

        /// <summary>
        /// Adds the coverage information of a single line of a file to the report.
        /// </summary>
        /// <param name="analysis">The line analysis.</param>
        public void LineAnalysis(LineAnalysis analysis)
        {
            if (analysis == null)
            {
                throw new ArgumentNullException("analysis");
            }

            string formattedLine = analysis.LineContent
                .Replace(((char)11).ToString(), "  ") // replace tab
                .Replace(((char)9).ToString(), "  "); // replace tab

            if (formattedLine.Length > 120)
            {
                formattedLine = formattedLine.Substring(0, 120);
            }

            formattedLine = WebUtility.HtmlEncode(formattedLine);
            formattedLine = formattedLine.Replace(" ", "&nbsp;");

            string lineVisitStatus = ConvertToCssClass(analysis.LineVisitStatus);

            this.reportTextWriter.Write("<tr data-coverage=\"{");

            this.reportTextWriter.Write(
                "'AllTestMethods': {{'VC': '{0}', 'LVS': '{1}'}}",
                analysis.LineVisitStatus != LineVisitStatus.NotCoverable ? analysis.LineVisits.ToString(CultureInfo.InvariantCulture) : string.Empty,
                lineVisitStatus);

            foreach (var coverageByTestMethod in analysis.LineCoverageByTestMethod)
            {
                this.reportTextWriter.Write(
                    ", 'M{0}': {{'VC': '{1}', 'LVS': '{2}'}}",
                    coverageByTestMethod.Key.Id.ToString(CultureInfo.InvariantCulture),
                    coverageByTestMethod.Value.LineVisitStatus != LineVisitStatus.NotCoverable ? coverageByTestMethod.Value.LineVisits.ToString(CultureInfo.InvariantCulture) : string.Empty,
                    ConvertToCssClass(coverageByTestMethod.Value.LineVisitStatus));
            }

            this.reportTextWriter.Write("}\">");

            this.reportTextWriter.Write(
                "<td class=\"{0}\">&nbsp;</td>",
                lineVisitStatus);
            this.reportTextWriter.Write(
                "<td class=\"leftmargin right\">{0}</td>",
                analysis.LineVisitStatus != LineVisitStatus.NotCoverable ? analysis.LineVisits.ToString(CultureInfo.InvariantCulture) : string.Empty);
            this.reportTextWriter.Write(
                "<td class=\"right\"><code>{0}</code></td>",
                analysis.LineNumber);
            this.reportTextWriter.Write(
                "<td{0}><code>{1}</code></td>",
                analysis.LineVisitStatus == LineVisitStatus.NotCoverable ? " class=\"light\"" : string.Empty,
                formattedLine);

            this.reportTextWriter.WriteLine("</tr>");
        }

        /// <summary>
        /// Finishes the current table.
        /// </summary>
        public void FinishTable()
        {
            this.reportTextWriter.WriteLine("</table>");
        }

        /// <summary>
        /// Adds the coverage information of an assembly to the report.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public void SummaryAssembly(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }

            this.reportTextWriter.WriteLine(
                "<tr class=\"expanded\"><th><a href=\"#\" class=\"toggleClassesInAssembly\" title=\"" + Resources.CollapseExpandAssembly + "\"> </a> {0} {1}</th><th title=\"{2}\">{3}%</th><td>{4}</td></tr>",
                WebUtility.HtmlEncode(assembly.Name),
                CreateAssemblyPopup(assembly),
                CoverageType.LineCoverage,
                assembly.CoverageQuota,
                CreateCoverageTable(assembly.CoverageQuota));
        }

        /// <summary>
        /// Adds the coverage information of a class to the report.
        /// </summary>
        /// <param name="class">The class.</param>
        public void SummaryClass(Class @class)
        {
            if (@class == null)
            {
                throw new ArgumentNullException("class");
            }

            this.reportTextWriter.WriteLine(
                "<tr class=\"classrow\"><td><a href=\"{0}\">{1}</a></td><td title=\"{2}\">{3}%</td><td>{4}</td></tr>",
                WebUtility.HtmlEncode(GetClassReportFilename(@class.Assembly.ShortName, @class.Name)),
                WebUtility.HtmlEncode(@class.Name),
                @class.CoverageType,
                @class.CoverageQuota,
                CreateCoverageTable(@class.CoverageQuota));
        }

        /// <summary>
        /// Saves a summary report.
        /// </summary>
        /// <param name="targetDirectory">The target directory.</param>
        public void SaveSummaryReport(string targetDirectory)
        {
            this.SaveReport();
            SaveCss(targetDirectory);
        }

        /// <summary>
        /// Saves a class report.
        /// </summary><param name="targetDirectory">The target directory.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="className">Name of the class.</param>
        public void SaveClassReport(string targetDirectory, string assemblyName, string className)
        {
            this.SaveReport();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.reportTextWriter != null)
                {
                    this.reportTextWriter.Dispose();
                }
            }
        }

        /// <summary>
        /// Builds a table showing the coverage quota with red and green bars.
        /// </summary>
        /// <param name="coverage">The coverage quota.</param>
        /// <returns>Table showing the coverage quota with red and green bars.</returns>
        private static string CreateCoverageTable(decimal coverage)
        {
            var stringBuilder = new StringBuilder();
            int covered = (int)Math.Round(coverage, 0);
            int uncovered = 100 - covered;

            if (covered == 100)
            {
                covered = 103;
            }

            if (uncovered == 100)
            {
                uncovered = 103;
            }

            stringBuilder.Append("<table class=\"coverage\"><tr>");
            if (covered > 0)
            {
                stringBuilder.Append("<td class=\"green\" style=\"width: " + covered + "px;\">&nbsp;</td>");
            }

            if (uncovered > 0)
            {
                stringBuilder.Append("<td class=\"red\" style=\"width: " + uncovered + "px;\">&nbsp;</td>");
            }

            stringBuilder.Append("</tr></table>");
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Builds a table containing information about an assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>Table containing information about an assembly</returns>
        private static string CreateAssemblyPopup(Assembly assembly)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("<a href=\"#\" class=\"toggleAssemblyDetails\" title=\"" + Resources.DetailsOfAssembly + "\">" + Resources.Details + "</a>");
            stringBuilder.AppendLine("<div class=\"detailspopup\">");
            stringBuilder.AppendLine("<table class=\"overview\">");
            stringBuilder.AppendLine("<colgroup>");
            stringBuilder.AppendLine("<col width=\"130\" />");
            stringBuilder.AppendLine("<col />");
            stringBuilder.AppendLine("</colgroup>");
            stringBuilder.AppendFormat("<tr><th>{0}</th><td>{1}</td></tr>", Resources.Classes, assembly.Classes.Count());
            stringBuilder.AppendFormat("<tr><th>{0}</th><td>{1}%</td></tr>", Resources.Coverage2, assembly.CoverageQuota);
            stringBuilder.AppendFormat("<tr><th>{0}</th><td>{1}</td></tr>", Resources.CoveredLines, assembly.CoveredLines);
            stringBuilder.AppendFormat("<tr><th>{0}</th><td>{1}</td></tr>", Resources.CoverableLines, assembly.CoverableLines);
            stringBuilder.AppendFormat("<tr><th>{0}</th><td>{1}</td></tr>", Resources.TotalLines, assembly.TotalLines);
            stringBuilder.AppendLine("</table>");
            stringBuilder.AppendLine("</div>");

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Converts the <see cref="LineVisitStatus"/> to the corresponding CSS class.
        /// </summary>
        /// <param name="lineVisitStatus">The line visit status.</param>
        /// <returns>The corresponding CSS class.</returns>
        private static string ConvertToCssClass(LineVisitStatus lineVisitStatus)
        {
            string css = "gray";

            if (lineVisitStatus == LineVisitStatus.Covered)
            {
                css = "green";
            }
            else if (lineVisitStatus == LineVisitStatus.NotCovered)
            {
                css = "red";
            }

            return css;
        }

        /// <summary>
        /// Gets the file name of the report file for the given class.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="className">Name of the class.</param>
        /// <returns>The file name.</returns>
        private static string GetClassReportFilename(string assemblyName, string className)
        {
            string key = assemblyName + "_" + className;

            string fileName = null;

            if (!FileNameByClass.TryGetValue(key, out fileName))
            {
                lock (FileNameByClass)
                {
                    if (!FileNameByClass.TryGetValue(key, out fileName))
                    {
                        string shortClassName = className.Substring(className.LastIndexOf('.') + 1);
                        fileName = RendererBase.ReplaceInvalidPathChars(assemblyName + "_" + shortClassName) + ".htm";

                        if (FileNameByClass.ContainsValue(fileName))
                        {
                            int counter = 2;

                            do
                            {
                                fileName = RendererBase.ReplaceInvalidPathChars(assemblyName + "_" + shortClassName + counter) + ".htm";
                                counter++;
                            } 
                            while (FileNameByClass.ContainsValue(fileName));
                        }

                        FileNameByClass.Add(key, fileName);
                    }
                }
            }

            return fileName;
        }

        /// <summary>
        /// Saves the CSS.
        /// </summary>
        /// <param name="targetDirectory">The target directory.</param>
        private static void SaveCss(string targetDirectory)
        {
            string targetPath = Path.Combine(targetDirectory, "report.css");

            try
            {
                System.IO.File.WriteAllText(targetPath, DefaultCss, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                string error = string.Format(CultureInfo.InvariantCulture, Resources.ReportNotSaved, targetPath, ex.Message);
                Logger.Error(error);
                throw new RenderingException(error, ex);
            }
        }

        /// <summary>
        /// Intitializes the text writer.
        /// </summary>
        /// <param name="targetPath">The target path.</param>
        private void CreateTextWriter(string targetPath)
        {
            try
            {
                this.reportTextWriter = new StreamWriter(new FileStream(targetPath, FileMode.Create));
            }
            catch (Exception ex)
            {
                string error = string.Format(CultureInfo.InvariantCulture, Resources.ReportNotSaved, targetPath, ex.Message);
                Logger.Error(error);
                throw new RenderingException(error, ex);
            }
        }

        /// <summary>
        /// Saves the report.
        /// </summary>
        private void SaveReport()
        {
            this.FinishReport();

            this.reportTextWriter.Flush();
            this.reportTextWriter.Dispose();

            this.reportTextWriter = null;
        }

        /// <summary>
        /// Finishes the report.
        /// </summary>
        private void FinishReport()
        {
            this.reportTextWriter.Write(string.Format(
                CultureInfo.InvariantCulture,
                "<div class=\"footer\">{0} {1} {2}<br />{3} - {4}<br /><a href=\"http://www.palmmedia.de\">www.palmmedia.de</a></div>",
                Resources.GeneratedBy,
                this.GetType().Assembly.GetName().Name,
                this.GetType().Assembly.GetName().Version,
                DateTime.Now.ToShortDateString(),
                DateTime.Now.ToLongTimeString()));

            this.reportTextWriter.Write(HtmlEnd);
        }
    }
}
