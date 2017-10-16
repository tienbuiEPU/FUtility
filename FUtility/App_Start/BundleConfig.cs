using System.Web;
using System.Web.Optimization;

namespace FUtility
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/angularjs")
                .Include("~/Scripts/angular.min.js",
                "~/Scripts/angular-mocks.js",
                "~/Scripts/angular-route.min.js",
                "~/Scripts/ngdialog.min.js",
                "~/Scripts/ng-file-upload.min.js",
                "~/Scripts/angular-md5.js",
                "~/Scripts/angular-masonry.min.js",
                "~/Scripts/angular-ui/ui-bootstrap.min.js",
                "~/Scripts/angular-ui/ui-bootstrap-tpls.min.js",
                "~/Scripts/angular-timeago.min.js",
                "~/Scripts/Chart.bundle.js",
                "~/Scripts/angular-chart.min.js",
                "~/Scripts/moment.js",
                "~/Scripts/datetimepicker.js",
                "~/Scripts/datetimepicker.templates.js",
                "~/Scripts/loading-bar.min.js",
                "~/Scripts/export/ngPrint.min.js",
                "~/Scripts/export/FileSaver.min.js",
                "~/Scripts/export/json-export-excel.min.js",
                "~/Scripts/angular-animate.min.js",
                "~/Scripts/angular-aria.min.js",
                "~/Scripts/angular-messages.min.js",
                "~/Scripts/angular-material.min.js",
                "~/Scripts/angular-upload.js",
                "~/Scripts/app/app.js"
                //"~/Scripts/app/upload.js"
                ));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
        }
    }
}
