using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using IAGrim.BuddyShare;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static IAGrim.BuddyShare.Synchronizer;
using SharpTestsEx;

namespace Tests_IAGrim {
    [TestClass]
    public class CompressionTests {

        string[] lorums = new[] {
            @"Vivamus at finibus ante, et faucibus tortor. Mauris facilisis ornare orci, at sodales metus varius eu. Fusce eget nunc a urna vestibulum auctor. Donec dolor purus, ornare eget accumsan et, vulputate sit amet justo. Morbi faucibus nibh in nisl pharetra rhoncus. Sed non odio ex. Praesent egestas elementum pellentesque. Aliquam orci mauris, vulputate at pharetra eget, semper eu tellus. Vivamus ex lectus, finibus vel nunc id, commodo vulputate enim. In metus sem, mollis et sapien vel, imperdiet viverra nibh. Curabitur libero lacus, imperdiet nec urna ut, feugiat fermentum eros. Nam rutrum turpis a elit volutpat, eget pulvinar risus pulvinar. Cras eu eros commodo, luctus tortor quis, accumsan risus.                ",
            @"Mauris venenatis vulputate arcu, ac tempor diam laoreet id. Donec erat felis, lobortis a ullamcorper nec, auctor non justo. Praesent tristique ante sed turpis sollicitudin, id faucibus magna iaculis. In mi diam, dapibus eget laoreet sed, ultricies non augue. Aliquam in ligula vitae nunc maximus aliquam. Sed dui elit, tempor ut aliquet at, ullamcorper a magna. Quisque arcu tellus, efficitur ut iaculis sed, pretium sit amet quam. Cras vel efficitur nulla. Proin a commodo odio, sed consectetur dui. Vestibulum mollis ante pulvinar nisl pellentesque dictum.",
            @"Vestibulum dignissim molestie nulla eu finibus. Vestibulum laoreet justo ac eros lobortis aliquam. Integer placerat dapibus maximus. Etiam consectetur eu massa quis consectetur. Pellentesque suscipit ultricies leo, ut aliquam nulla sollicitudin eu. Nam dignissim sed tortor sit amet efficitur. Nam non rutrum felis. Suspendisse vel justo ipsum. Nam lobortis consequat ante, sit amet hendrerit nisi lacinia quis. Sed eu erat eget nulla condimentum sagittis ut non ante. Aliquam libero ex, imperdiet a ultrices sed, malesuada nec sem.",
            @"Cras interdum eu diam eu mollis. Duis feugiat ipsum sed rhoncus malesuada. Etiam at tempus nulla, sed bibendum nibh. Sed quis finibus lorem. Curabitur venenatis cursus turpis, vitae tincidunt neque ultricies eget. Ut non nibh in nunc aliquet faucibus. Vestibulum pretium ipsum vitae quam luctus efficitur. In hac habitasse platea dictumst. Suspendisse potenti. Nunc aliquet justo at lorem tempus, eu molestie leo venenatis. Nam ex purus, dignissim eleifend dolor et, elementum faucibus magna. Aliquam bibendum lobortis ex. Morbi finibus mauris ut justo posuere luctus vel sit amet velit. Phasellus efficitur enim in sem faucibus placerat. Quisque rhoncus dui sit amet dapibus eleifend. Phasellus quis suscipit dui."
        };
        [TestMethod]
        public void TestDecompressPhpGeneratedData() {
            var data = "H4sIAAAAAAAAAyvJSFVIzUvOT8nMS1dILChILErMK8mpVCjPL8pOTQEAw6AGlB4AAAA=";
            var decoded = GzipCompressionHandler.DecodeBase64AndDecompress(data);
            decoded.Should().Be.EqualTo("the encoding apparantly worked");
        }

        //[TestMethod]
        public void TestDecompressPhpGeneratedData123() {
            foreach (var data in lorums) {
                var urlEncoded = HttpUtility.UrlEncode(data);
                using (WebClient client = new WebClient()) {
                    var url = UrlBuddyServer + "/test_encode_with_data?data=" + urlEncoded;
                    var result = client.DownloadString(url);

                    var decoded = GzipCompressionHandler.DecodeBase64AndDecompress(result);
                    decoded.Should().Be.EqualTo(data);
                }
            }
        }

      [TestMethod]
        public void TestCompression() {
            foreach (var lorum in lorums) {
                var e = GzipCompressionHandler.CompressAndConvertToBase64(lorum);
                var d = GzipCompressionHandler.DecodeBase64AndDecompress(e);

                Debug.Assert(d.Equals(lorum));
            }
        }
    }
}
