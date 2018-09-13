using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;

namespace Wboy.Infrastructure.Core.AliLib
{
    /// <summary>
    /// 阿里云发送短信验证码
    /// </summary>
    public class SmsSenderAli
    {
        //产品名称:云通信短信API产品,开发者无需替换
        private const string Product = "Dysmsapi";
        //产品域名,开发者无需替换
        private const string Domain = "dysmsapi.aliyuncs.com";

        private readonly string _accessId;
        private readonly string _accessSecret;
        private readonly string _signName;
        private readonly string _smsTemplateCode;
        //private const string AccessId = "LTAI4yYSeH43LQU0";
        //private const string AccessKey = "Hc1zscUFhy323rG0pEJScJZbVJoL4A";
        
        /// <summary>
        /// 自己实现缓存
        /// </summary>
        //private readonly ICache _cache;///

        //public SmsSenderAli(ICache cache)
        //{
        //    var setting = ConfigurationManager.GetSetting<SettingAliyun>();
        //    _accessId = setting.OssAccessId;
        //    _accessSecret = setting.OssAccessSecret;
        //    _signName = setting.SmsSignName;
        //    _smsTemplateCode = setting.SmsTemplateCode;
        //    _cache = cache;
        //}

        public string SendSmsCode(string phone)
        {
            return SendSmsCode(phone, phone);
        }

        public string SendSmsCode(string phone,string cacheKey)
        {
            ///var verCode = _cache.Get<VerificationCode>(cacheKey); 获取缓存的验证码
            //if (verCode != null)
            //{
            //    if ((DateTime.Now - verCode.SendTime).TotalSeconds < 600)
            //    {
            //        throw new Exception("验证码已发送！请不要重复获取");
            //    }
            //}
            string code = "123456";
            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", _accessId, _accessSecret);
            DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", Product, Domain);
            IAcsClient acsClient = new DefaultAcsClient(profile);
            SendSmsRequest request = new SendSmsRequest();
            try
            {
                //必填:待发送手机号。支持以逗号分隔的形式进行批量调用，批量上限为1000个手机号码,批量调用相对于单条调用及时性稍有延迟,验证码类型的短信推荐使用单条调用的方式
                request.PhoneNumbers = phone;
                //必填:短信签名-可在短信控制台中找到
                request.SignName = _signName;
                //必填:短信模板-可在短信控制台中找到
                request.TemplateCode = _smsTemplateCode;
                //可选:模板中的变量替换JSON串,如模板内容为"亲爱的${name},您的验证码为${code}"时,此处的值为
                request.TemplateParam = $"{{\"code\":\"{code}\"}}";
                //可选:outId为提供给业务方扩展字段,最终在短信回执消息中将此值带回给调用者
                request.OutId = "yourOutId";
                //请求失败这里会抛ClientException异常
                var response = acsClient.GetAcsResponse(request);
                //verCode = new VerificationCode() { Code = code, SendTime = DateTime.Now };
                //_cache.Set(cacheKey, verCode, 600);
                return code;
            }
            //catch (ServerException ex)
            //{

            //}
            catch (ClientException ex)
            {

                switch (ex.ErrorCode)
                {
                    case "InvalidDayuStatus.Malformed":
                        throw new Exception("账户短信开通状态不正确");
                    case "InvalidSignName.Malformed":
                        throw new Exception("短信签名不正确");
                    case "InvalidTemplateCode.Malformed":
                        throw new Exception("模板状态不正确");
                    case "InvalidRecNum.Malformed":
                        throw new Exception("验证码发送失败！手机号不正确");
                    case "InvalidParamString.Malformed":
                        throw new Exception("短信模板中变量不是json格式");
                    case "InvalidParamStringTemplate.Malformed":
                        throw new Exception("短信模板中变量与模板内容不匹配");
                    case "InvalidSendSms":
                        //throw new NeedToShowFrontException("验证码发送失败！触发业务流控");
                        throw new Exception("验证码发送失败！请求太频繁");
                    case "InvalidDayu.Malformed":
                        throw new Exception("变量不能是url，可以将变量固化在模板中");
                }
                throw new Exception("验证码发送失败！请稍后再试");
            }
            catch (Exception ex)
            {

                throw new Exception("验证码发送失败！请稍后再试");
            }
        }
        public bool ValidateSmsCode(string cacheKey, string code, bool deleteKey = true)
        {
            //            return true;
            //if (_cache.Get<VerificationCode>(cacheKey)?.Code == code)
            //{
            //    //验证成功移除
            //    if (deleteKey)
            //    {
            //        _cache.Remove(cacheKey);
            //    }
            //    return true;
            //}
            return false;
        }

        public string GetSmsCode(string cacheKey)
        {
            return "111";
            //return _cache.Get<VerificationCode>(cacheKey)?.Code;
        }
    }
}
