# 工程说明
阿里云官方sdk没有提供.net core版本的库

这里根据官方的修改移植了.net core版本

官方版本地址：

https://github.com/aliyun/aliyun-openapi-net-sdk

https://github.com/aliyun/aliyun-oss-csharp-sdk?spm=a2c4g.11186623.2.8.402917a5Y5wgEb



主要修改点

- 修改点
  Aliyun.Acs.Core.Regions.InternalEndpointsParser

这个类下面的 LoadEndpointDocument函数
assembly.GetManifestResourceStream(resourceName);
会加载不出来数据流
建议直接修改为

```
string resourceName = AppDomain.CurrentDomain.BaseDirectory + "Regions\\" + BUNDLED_ENDPOINTS_RESOURCE_PATH;
XmlDocument xmlDoc = new XmlDocument();
xmlDoc.Load(resourceName);
```

方式方式XmlDocument

- 修改点 ShaHmac1 加密算法

```
            var keyBytes = Encoding.UTF8.GetBytes(accessSecret);
            var data = Encoding.UTF8.GetBytes(source);
            var hmac = new HMACSHA1(keyBytes);
            byte[] bytes = hmac.ComputeHash(data);
            return Convert.ToBase64String(bytes);
```

- 修改点
  Aliyun.Acs.Core.Http.HttpResponse

注释掉

```
//httpWebRequest.ServicePoint.Expect100Continue = false;
```