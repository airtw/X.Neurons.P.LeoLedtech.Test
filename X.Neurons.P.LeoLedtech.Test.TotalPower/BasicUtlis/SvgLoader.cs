using DevExpress.Utils;
using DevExpress.Utils.Svg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Neurons.P.LeoLedtech.Test.TotalPower.BasicUtlis
{
    public static class SvgLoader
    {
        /// <summary>
        /// 將資料夾中的 .svg 檔載入到 SvgImageCollection
        /// </summary>
        /// <param name="collection">目標 SvgImageCollection</param>
        /// <param name="folder">資料夾路徑</param>
        /// <param name="recursive">是否包含子資料夾</param>
        /// <param name="clearExisting">載入前是否先清空集合</param>
        /// <param name="namePrefix">加在 key 前的字首（可為空）</param>
        public static void LoadFolder(
            SvgImageCollection collection,
            string folder,
            bool recursive = false,
            bool clearExisting = true,
            string namePrefix = "")
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
                throw new DirectoryNotFoundException($"Folder not found: {folder}");

            if (clearExisting) collection.Clear();

            var option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (var path in Directory.EnumerateFiles(folder, "*.svg", option))
            {
                try
                {
                    // 用檔名（不含副檔名）當作 key
                    var key = namePrefix + Path.GetFileNameWithoutExtension(path);

                    // 若 key 重複，附加序號避免例外
                    var uniqueKey = key;
                    int i = 1;
                    while (collection.ContainsKey(uniqueKey))
                        uniqueKey = $"{key}_{i++}";

                    var svg = SvgImage.FromFile(path);
                    collection.Add(uniqueKey, svg);

                    //（選配）若想統一顏色化行為，可設定：
                    // collection[uniqueKey].ColorizationMode = SvgColorizationMode.CommonPalette;
                }
                catch (Exception ex)
                {
                    // 依需求處理：可記 log、忽略壞檔案等
                    System.Diagnostics.Debug.WriteLine($"Failed to load SVG '{path}': {ex.Message}");
                }
            }
        }
    }
}
