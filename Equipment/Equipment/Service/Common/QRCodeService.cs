using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Equipment.Service.Common
{
    public class QRCodeService: IBaseService
    {
        private QRCodeGenerator _generator;
        public QRCodeService()
        {
            _generator = new QRCodeGenerator();
        }
        /// <summary>
        /// 普通二维码
        /// </summary>
        /// <param name="url">存储内容</param>
        /// <param name="pixel">像素大小</param>
        /// <returns></returns>
        public Bitmap GetQRCode(string url, int pixel)
        {
            QRCodeData codeData = _generator.CreateQrCode(url, QRCodeGenerator.ECCLevel.M, true);
            QRCode qrcode = new QRCode(codeData);
            Bitmap qrImage = qrcode.GetGraphic(pixel, Color.Black, Color.White, true);
            return qrImage;
        }


        /// <summary>
        /// SVG二维码
        /// </summary>
        /// <param name="url"></param>
        /// <param name="pixel"></param>
        /// <returns></returns>
        public string GetSvgQRCode(string url, int pixel)
        {
            var qrCodeData = _generator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new SvgQRCode(qrCodeData);
            return qrCode.GetGraphic(pixel);
        }


    }
}
