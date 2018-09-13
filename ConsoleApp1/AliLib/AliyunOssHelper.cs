using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Aliyun.OSS;

namespace Wboy.Infrastructure.Core.AliLib
{
    public class AliyunOssHelper
    {
        private readonly string _endpoint;

        private readonly string _bucketName;

        private readonly OssClient _ossClient;
         
        public AliyunOssHelper()
        {
            //To Do自己实现初始化
            //var setting = ConfigurationManager.GetSetting<SettingAliyun>();获取配置
            //var accessId = setting.OssAccessId;
            //var accessSecret = setting.OssAccessSecret;
            //_endpoint = setting.OssEndpoint;
            //_bucketName = setting.OssBucketName;
            //_ossClient = new OssClient(_endpoint, accessId, accessSecret);
        }

        public AliyunOssHelper(string accessId, string accessSecret, string securityToken)
        {
            _ossClient = new OssClient(_endpoint, accessId, accessSecret, securityToken);
        }
        public Uri GetGeneratePresignedUri(string fileKey, DateTime expiration)
        {
            try
            {
                var req = new GeneratePresignedUriRequest(_bucketName, fileKey, SignHttpMethod.Get)
                {
                    Expiration = expiration
                };
                return _ossClient.GeneratePresignedUri(req);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// 创建一个文件夹 
        /// </summary>
        /// <param name="folderKey">yourfolder/以/结尾</param>
        public void CreateFolder(string folderKey)
        {
            // 初始化OssClient
            // Note: key treats as a folder and must end with a slash.
            //const string yourfolder = "yourfolder/";
            // put object with zero bytes stream.
            using (MemoryStream ms = new MemoryStream())
            {
                _ossClient.PutObject(_bucketName, folderKey, ms);
            }
        }

        /// <summary>
        /// 上传指定的文件到指定的OSS的存储空间
        /// </summary>
        /// <param name="key">文件的在OSS上保存的名称</param>
        /// <param name="hostFilePath">指定上传文件的本地路径</param>
        public bool PutObject(string key, string hostFilePath)
        {
            var result = _ossClient.PutObject(_bucketName, key, hostFilePath);
            if (result.HttpStatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            LoggerFactory.CreateLoger().Warning("Aliyun Get PutObject failed, {0}", result.ETag);
            return false;
        }

        public bool PutObject(string key, Stream content)
        {
            var result = _ossClient.PutObject(_bucketName, key, content);
            if (result.HttpStatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            LoggerFactory.CreateLoger().Warning("Aliyun Get PutObject failed, {0}", result.ETag);
            return false;
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="objKey">对象key 路径</param>
        public void DeleteObject(string objKey)
        {
            _ossClient.DeleteObject(_bucketName, objKey);
        }

        public void DeleteObjects(IList<string> objKeylist)
        {
            if (objKeylist?.Count > 0)
            {
                var deleteObjectsRequest = new DeleteObjectsRequest(_bucketName, objKeylist);
                var result = _ossClient.DeleteObjects(deleteObjectsRequest);
                // return result;
            }
        }

        /// <summary>
        /// 复制对象
        /// </summary>
        /// <param name="sourceKey">源Key路径</param>
        /// <param name="destinationKey">新Key路径</param>
        public void CopyObject(string sourceKey,string destinationKey)
        {
            var copyobjectrequest = new CopyObjectRequest(_bucketName, sourceKey, _bucketName, destinationKey);
            var copyObjectResult = _ossClient.CopyObject(copyobjectrequest);
        }
        /// <summary>
        /// 从指定的OSS存储空间中获取指定的文件
        /// </summary>
        /// <param name="key">要获取的文件在OSS上的名称</param>
        /// <param name="hostDownloadPath">本地存储下载文件的目录</param>
        public bool GetObject(string key, string hostDownloadPath)
        {
            try
            {
                var result = _ossClient.GetObject(_bucketName, key);

                //将从OSS读取到的文件写到本地
                using (var requestStream = result.Content)
                {
                    byte[] buf = new byte[1024];
                    using (var fs = File.Open(hostDownloadPath, FileMode.OpenOrCreate))
                    {
                        var len = 0;
                        while ((len = requestStream.Read(buf, 0, 1024)) != 0)
                        {
                            fs.Write(buf, 0, len);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LoggerFactory.CreateLoger().Warning("Aliyun Get object failed, {0}", ex.Message);
               //Console.WriteLine("Get object failed, {0}", ex.Message);
               // throw new NeedToShowFrontException(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key">要获取的文件在OSS上的名称</param>
        /// <returns></returns>
        public bool IsExistObject(string key)
        {
            try
            {
                return _ossClient.DoesObjectExist(_bucketName, key);
            }
            catch (Exception ex)
            {
                LoggerFactory.CreateLoger().Warning("Aliyun DoesObjectExist failed, {0}", ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 获取文件大小 KB
        /// </summary>
        /// <param name="key">要获取的文件在OSS上的名称</param>
        public int GetObjectFileSize(string key)
        {
            try
            {

                var size = 0;
                var result = _ossClient.GetObject(_bucketName, key);
                if (result != null)
                {
                    using (var requestStream = result.Content)
                    {
                        byte[] buf = new byte[1024];
                        using (var fs = new MemoryStream())
                        {
                            var len = 0;
                            while ((len = requestStream.Read(buf, 0, 1024)) != 0)
                            {
                                fs.Write(buf, 0, len);
                            }
                            size = (int)fs.Length;
                        }
                    }
                }
                return size/1024;

            }
            catch (Exception ex)
            {
                LoggerFactory.CreateLoger().Warning("Aliyun Get object failed, {0}", ex.Message);
                return 0;
            }
        }
    }
}
