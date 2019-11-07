# fastdfs 安装教程

## 下载与安装必要软件

- 必要软件:
  - 安装必要的软件:`yum -y install zlib zlib-devel pcre pcre-devel gcc gcc-c++ openssl openssl-devel libevent libevent-devel perl unzip net-tools wget`

- 创建目录
  
  - 定位目录: `cd /usr/local`
  - 创建下载目录: `mkdir fast_download`

- 安装libfastcommon
  
  - 下载libfastcommon: `wget https://github.com/happyfish100/libfastcommon/archive/V1.0.41.zip`
  - 解压libfastcommon: `unzip V1.0.41.zip` 得到文件目录 `libfastcommon-1.0.41`
  - 安装:
    - 定位到目录 `cd libfastcommon-1.0.41`,运行 `./make.sh`
    创建软连接:
      - `ln -s /usr/lib64/libfastcommon.so /usr/local/lib/libfastcommon.so`
      - `ln -s /usr/lib64/libfastcommon.so /usr/lib/libfastcommon.so`
      - `ln -s /usr/lib64/libfdfsclient.so /usr/local/lib/libfdfsclient.so`
      - `ln -s /usr/lib64/libfdfsclient.so /usr/lib/libfdfsclient.so`
    - 运行 `./make.sh install`

- 安装fastdfs

  - 下载fastdfs: `wget https://github.com/happyfish100/fastdfs/archive/V6.01.tar.gz`
  - 解压fastdfs: `tar zxvf V6.01.tar.gz`,得到文件目录 `fastdfs-6.01`
  - 安装: 定位到fastdfs解压出来的目录,运行 `./make.sh`,执行完成后再运行 `./make.sh install`
  - 配置： 定位到fast运行目录 `cd /etc/fdfs` (正常安装完成后,会自动创建 `/etc/fdfs` 目录,该目录是固定的)
  - 配置文件: `cd /etc/fdfs`,在该目录中执行
    - `mv tracker.conf.sample tracker.conf`
    - `mv client.conf.sample client.conf`
    - `mv storage.conf.sample storage.conf`
  
- Tracker与Storage
  > `Tracker` 与 `Storage` 他们之间的安装,在此以上的步骤都是相同的,如果他们是在不同的机器上安装,只需要按照上面的步骤分别在机器上操作即可。
  - Tracker服务器:
    - Tracker配置: Tracker需要配置 `/etc/fdfs/tracker.conf` 文件
    - Tracker主要配置参数说明: `bind_addr`(本地地址) , `port`(端口号,默认22122) , `base_path`(Tracker存放日志等基础目录,该目录必须先创建好) , `store_group`(Tracker对应的存储的组名,必须与安装的Storage的组相对应)
    - 运行Tracker: `fdfs_trackerd /etc/fdfs/tracker.conf`
  - Storage服务器:
    - Storage配置: Storage需要配置 `/etc/fdfs/storage.conf` 文件
    - Storage主要配置参数说明: `group_name`(组名,需要与Tracker中的组名相对应) , `bind_addr`(Storage本地地址), `port` (Storage端口号) `base_path` (Storage存放日志等基础目录,该目录必须先创建好) , `store_path0`(真正存放存储文件的目录,必须先创建好,可以配置多个) , `tracker_server`(同组内的Tracker ip地址与端口,必须与Tracker对应)
    - 运行Storage: `fdfs_storaged /etc/fdfs/storage.conf`,如果正常运行,且配置没有错误, Storage配置的 `store_path0` 目录下面会创建2级目录,每级目录256个
  - 查看运行的fastdfs进程: `ps -ef| grep fdfs` ,以及 `fdfs_monitor` 命令
  - 结束进程: 可使用 `killall fdfs_trackerd` 结束Tracker进程 , 可使用 `killall fdfs_storaged` 结束Storage进程
  - Client配置,用来临时测试
    - Client配置: Client需要配置 `client.conf`
    - Client主要参数说明:  `base_path`(Client存放日志等基础目录,该目录必须先创建好), `tracker_server`(Tracker的地址,必须与Tracker对应)
    - 用Client测试Tracker与Storage: `fdfs_upload_file client.conf /usr/local/fast_download/V1.0.41.zip` (测试上传的客户端可以自己定义),如果上传成功会出现类似的路径  **`group1/M00/00/00/wKgBcFzd0FaAID6HAAMmG0YMmVg.41.zip`**
  
  > 上传文件之后,可以到对应的Storage服务器上,查看存储目录,如 `store_path0` 参数对应目录下的文件是否存在

- **到此为止,fastdfs的基本功能已经安装完成,客户端可以使用相应的sdk进行正常的操作了,把它当作一个存储服务器来用**

## fastdfs nginx安装

- fastdfs nginx 基础安装:
  - 下载,解压nginx: `wget http://nginx.org/download/nginx-1.17.1.tar.gz`, `tar zxvf nginx-1.17.1.tar.gz`
  - 下载,解压fastdfs-nginx-module: `wget https://github.com/happyfish100/fastdfs-nginx-module/archive/V1.21.tar.gz`, `tar zxvf V1.21.tar.gz`
  - 版本编译时的设置:
    - 进入到fastdfs-nginx-module目录,`cd fastdfs-nginx-module-1.21/src`
    - 修改config文件:

      ```json
      ngx_module_incs="/usr/include/fastdfs /usr/include/fastcommon/"
      CORE_INCS="$CORE_INCS /usr/include/fastdfs /usr/include/fastcommon/"
      ```

  - 拷贝必要的文件:
    - `cp /usr/local/fast_download/fastdfs-6.01/conf/http.conf /etc/fdfs` (该文件在fastdfs源文件中的conf目录下)
    - `cp /usr/local/fast_download/fastdfs-6.01/conf/mime.types /etc/fdfs` (该文件在fastdfs源文件中的conf目录下)
    - `cp /usr/local/fast_download/fastdfs-nginx-module-1.20/src/mod_fastdfs.conf /etc/fdfs/` (该文件在下载的fastdfs-nginx-module的src目录下)
  - 修改mod_fastdfs.conf文件

  ```json
  base_path=/usr/local/fastdfs/storage
  #tracker地址
  tracker_server=192.168.0.129:22122
  store_path0=/usr/local/fastdfs/storage_save
  #url是否包含group
  url_have_group_name = true
  ```

- storage nginx 安装:
  - storage nginx 编译设置: `cd /usr/local/fast_download/nginx-1.17.1`,`./configure --prefix=/usr/local/nginx_storage --add-module=/usr/local/fast_download/fastdfs-nginx-module-1.20/src`
  - 编译&安装: `make`, `make install`
  - storage nginx配置
    - 定位到 storage nginx安装目录下,`cd /usr/local/nginx_storage`
    - 编辑配置文件`vim conf/nginx.conf`,添加location节点如下,其中 root的内容为 storage保存存储文件的路径(store_path0,store_path1...)

    ``` json
    location /group1/M00 {
        root /usr/local/fastdfs/storage_save;
        ngx_fastdfs_module;
    }

    #如果存在多个 store_path
    location /group1/M01 {
        root /usr/local/fastdfs/storage_save2;
        ngx_fastdfs_module;
    }
    ```

    - 添加软链接: `ln  -s  /usr/local/fastdfs/storage_save/data /usr/local/fastdfs/storage_save/data/M00`
    - 启动nginx: `/usr/local/nginx_storage/sbin/nginx`
  
- tracker nginx安装:
  - tracker nginx 编译设置: `cd /usr/local/fast_download/nginx-1.17.1`,`./configure --prefix=/usr/local/nginx_tracker --add-module=/usr/local/fast_download/fastdfs-nginx-module-1.20/src`
  - 编译&安装: `make`, `make install`
  - storage nginx配置
    - 定位到 tracker nginx安装目录下,`cd /usr/local/nginx_tracker`
    - 编辑配置文件

    ``` json
    upstream fdfs_group1 {
        #storage nginx的ip与端口
        server 192.168.0.129:8080;
    }

    #也可以用如下方式配置
    #upstream fdfs_group1 {
    #    server 10.195.0.23:1789 weight=1 max_fails=2 fail_timeout=30s;
    #    server 10.195.0.25:1789 weight=1 max_fails=2 fail_timeout=30s;
    #}

    #ip地址端口号等配置
    server {
        listen       8081;
        server_name  192.168.0.129;
        location /group1/M00 {
            proxy_pass http://fdfs_group1;
        }
    }

    ```

    - 启动nginx: `/usr/local/nginx_tracker/sbin/nginx`

## 问题解决方案

- > 一些低版本的linux在安装fastdfs时会出现 '未找到命令的错误',这时候可以运行 `yum -y install zlib zlib-devel pcre pcre-devel gcc gcc-c++ openssl openssl-devel libevent libevent-devel perl unzip net-tools wget` 安装一些相关的配置组件

- > 安装好组件后,可能还会出现 `undefined reference to 'g_exe_name'` 这样的错误,可以运行 `./make.sh clean` 清理下。

## 防火墙配置

- 启动： `systemctl start firewalld`
- 关闭： `systemctl stop firewalld`
- 查看状态： `systemctl status firewalld`
- 开机禁用： `systemctl disable firewalld`
- 开机启用： `systemctl enable firewalld`
- 重新载入:  `firewall-cmd --reload`
- 查看:  `firewall-cmd --zone= public --query-port=80/tcp`
- 添加端口号:  `firewall-cmd --zone=public --add-port=80/tcp --permanent`    （--permanent永久生效，没有此参数重启后失效）

## 一键安装脚本

- [脚本](../install/install.sh)

- 使用

```shell
> wget https://github.com/cocosip/FastDFSCore/blob/master/install/install.sh

# 格式化脚本,在windows下编辑的脚本需要格式化一下才能在linux下正常运行
> sed -i 's/\r$//' install.sh
> ./install.sh

```
