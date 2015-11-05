(function (context) {
    var findExportImportAdminPlugin = (function () {
        var ids;

        var updateCounts = function(site, language) {
            $.ajax({
                type: "POST",
                url: "FindExportImportApi.asmx/GetCounts",
                data: JSON.stringify({ siteId: site, language: language }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function(msg) {
                    $.each(msg.d, function(inex, item) {
                        var link = $("a[id$=" + item.EntityKey + "]");
                        link.text("(" + item.Count + ")");
                    });
                }
            });
        };

        var updateExportCounts = function() {
            var site = $("#" + ids.exportSite).val();
            var language = $("#" + ids.exportLanguage).val();
            if (site && language) {
                updateCounts(site, language);
            }
        };

        var updateDeleteCounts = function() {
            var site = $("#" + ids.deleteSite).val();
            var language = $("#" + ids.deleteLanguage).val();
            if (site && language) {
                updateCounts(site, language);
            }
        };

        var init = function (elementIds) {
            if (!elementIds) {
                throw "elementIds argument is required";
            }
            ids = elementIds;
            $("#" + ids.confirmUnderstand).change(function () {
                if ($(this).is(":checked")) {
                    $("#" + ids.deleteButton).removeAttr("disabled");
                } else {
                    $("#" + ids.deleteButton).attr("disabled", "disabled");
                }
            });

            $("#" + ids.exportButton).click(function () {
                $("#" + ids.exportResultPanel).hide();
            });
            $("#" + ids.deleteButton).attr("disabled", "disabled");

            $("#" + ids.exportSite).change(updateExportCounts);
            $("#" + ids.exportLanguage).change(updateExportCounts);

            $("#" + ids.deleteSite).change(updateDeleteCounts);
            $("#" + ids.deleteLanguage).change(updateDeleteCounts);

            updateExportCounts();
            updateDeleteCounts();
        };

        return {
            init: init
        }

    })();
    
    context.epi = context.epi || {};
    context.epi.findExportImportPlugin = findExportImportAdminPlugin;
  
})(window)
