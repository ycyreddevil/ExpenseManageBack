using System.Security.Cryptography;
using System.Text;
using System;
namespace yuyu.Infrastructure
{
    public class Encrypt
    {
        /// <summary>
        /// 密码加密
        /// </summary>
        /// <param name="password">密码</param>
        /// <param name="mobile">手机号</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string encryptPassword(string password,string mobile)
        {
            var result = sha256(mobile + password);
            result = Md5(result);
            result = sha256(result);
            return result;
        }
        /// <summary>
        /// sh256加密方式
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string sha256(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = SHA256Managed.Create().ComputeHash(bytes);
 
            var builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("X2"));
            }
 
            return builder.ToString();
        }
        /// <summary>
        /// md5加密方式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string Md5(string str)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] byteArray = Encoding.UTF8.GetBytes(str);
            byteArray = md5.ComputeHash(byteArray);
           
            string hashedValue = "";
            foreach (byte b in byteArray)
            {
                hashedValue += b.ToString("x2");
            }
            return hashedValue; 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GenerateRandomCode()
        {
            var result = Guid.NewGuid().ToString("D");           
            return result;
        }
        
        /// <summary>
        /// 唯一订单号生成
        /// </summary>
        /// <returns></returns>
        public static string GenerateDocCode()
        {
            var strDateTimeNumber = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            var strRandomResult = NextRandom(1000, 1).ToString("0000");

            return strDateTimeNumber + strRandomResult;
        }
        
        /// <summary>
        /// 参考：msdn上的RNGCryptoServiceProvider例子
        /// </summary>
        /// <param name="numSeeds"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static int NextRandom(int numSeeds, int length)
        {
            byte[] randomNumber = new byte[length];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomNumber);
            uint randomResult = 0x0;
            for (int i = 0; i < length; i++)
            {
                randomResult |= ((uint) randomNumber[i] << ((length - 1 - i)*8));
            }

            return (int) (randomResult%numSeeds) + 1;
        }
        
        /// <summary>
        /// 随机验证码生成器（后续需要改成短信发送接口）
        /// </summary>
        /// <returns></returns>
        public static string GenerateRandomNumber(int length)
        {
            var result = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                var r = new Random(Guid.NewGuid().GetHashCode());
                result.Append(r.Next(0, 10));
            }
            return result.ToString();
        }
    }
}