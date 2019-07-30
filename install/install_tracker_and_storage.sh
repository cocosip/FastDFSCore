#!/bin/bash
echo -e "[--------------------fastdfs install--------------------]"
# sed -i 's/\r$//' install.sh

# 安装必要软件
yum -y install zlib zlib-devel pcre pcre-devel gcc gcc-c++ openssl openssl-devel libevent libevent-devel perl unzip net-tools wget

# 软件所在目录
software_path="/usr/local/fastdfs_download"
# tracker运行目录
tracker_base_path="/usr/local/fastdfs/tracker"
# tracker nginx目录
tracker_nginx_path="/usr/local/nginx/tracker_nginx"
# storage运行目录
storage_base_path="/usr/local/fastdfs/storage"
# storage nginx目录
storage_nginx_path="/usr/local/nginx/storage_nginx"
# storage文件保存的路径
storage_data_path="/usr/local/fastdfs/storage_data"

# 当前组
group_name="group1";
# 客户端运行目录
client_base_path="/usr/local/fastdfs/client";

# 当前ip
local_ip=`/sbin/ifconfig -a|grep inet|grep -v 127.0.0.1|grep -v inet6|awk '{print $2}'|tr -d "addr:"`

# 创建软件目录
if [ ! -d ${software_path} ] ;then
	mkdir -p ${software_path}
fi
# 创建tracker目录
if [ ! -d ${tracker_base_path} ] ;then
	mkdir -p ${tracker_base_path}
fi
# 创建tracker nginx目录
if [ ! -d ${tracker_nginx_path} ] ;then
	mkdir -p ${tracker_nginx_path}
fi
# 创建storage目录
if [ ! -d ${storage_base_path} ] ;then
	mkdir -p ${storage_base_path}
fi
# 创建storage文件目录
if [ ! -d ${storage_data_path} ] ;then
	mkdir -p ${storage_data_path}
fi
# 创建client目录
if [ ! -d ${client_base_path} ] ;then
	mkdir -p ${client_base_path}
fi
# 下载软件包
echo -e "[--------------------download soft--------------------]"
wget -P ${software_path} https://github.com/happyfish100/libfastcommon/archive/V1.0.39.zip
wget -P ${software_path} https://github.com/happyfish100/fastdfs/archive/V5.11.tar.gz
wget -P ${software_path} http://nginx.org/download/nginx-1.17.1.tar.gz
wget -P ${software_path} https://github.com/happyfish100/fastdfs-nginx-module/archive/V1.20.tar.gz

cd ${software_path}
# 解压软件
unzip V1.0.39.zip
tar zxvf V5.11.tar.gz
tar zxvf nginx-1.17.1.tar.gz
tar zxvf V1.20.tar.gz

# 安装libfastcommon
cd libfastcommon-1.0.39
./make.sh || exit 1
./make.sh install || exit 1

# 创建软链接
ln -s /usr/lib64/libfastcommon.so /usr/local/lib/libfastcommon.so
ln -s /usr/lib64/libfastcommon.so /usr/lib/libfastcommon.so
ln -s /usr/lib64/libfdfsclient.so /usr/local/lib/libfdfsclient.so
ln -s /usr/lib64/libfdfsclient.so /usr/lib/libfdfsclient.so

# 安装fastdfs
cd ..
cd fastdfs-5.11
./make.sh || exit 1
./make.sh install || exit 1

cd /etc/fdfs
# 拷贝配置文件
mv client.conf.sample client.conf
mv storage.conf.sample storage.conf
mv tracker.conf.sample tracker.conf
# http.conf和mime.types,mod_fastdfs.conf,到/etc/fdfs下
cp ${software_path}"/fastdfs-5.11/conf/http.conf" /etc/fdfs/
cp ${software_path}"/fastdfs-5.11/conf/mime.types" /etc/fdfs/
cp ${software_path}"/fastdfs-nginx-module-1.20/src/mod_fastdfs.conf" /etc/fdfs/

echo -e "[--------------------config conf--------------------]"
# 修改tracker.conf配置
# 配置ip地址
sed -i "/^bind_addr=/c\bind_addr=$local_ip" tracker.conf

# 配置tracker运行根目录
sed -i "/^base_path=/c\base_path=$tracker_base_path" tracker.conf
# 配置store group
sed -i "/^store_group=/c\store_group=$group_name" tracker.conf

# 修改storage.conf配置
# 配置group
sed -i "/^group_name=/c\group_name=$group_name" storage.conf
# 配置ip地址
sed -i "/^bind_addr=/c\bind_addr=$local_ip" storage.conf
# 配置storage运行根目录
sed -i "/^base_path=/c\base_path=$storage_base_path" storage.conf
# 配置磁盘0的目录
sed -i "/^store_path0=/c\store_path0=$storage_data_path" storage.conf
# 配置tracker ip地址
sed -i "/^tracker_server=/c\tracker_server=$local_ip:22122" storage.conf

# 修改client.conf配置
# 配置client运行根目录
sed -i "/^base_path=/c\base_path=$client_base_path" client.conf
# 配置tracker ip地址
sed -i "/^tracker_server=/c\tracker_server=$local_ip:22122" client.conf

# fastdfs-nginx-module配置
ngx_module_incs_path="/usr/include/fastdfs /usr/include/fastcommon/"
core_incs_path="\$CORE_INCS /usr/include/fastdfs /usr/include/fastcommon/"
# 
cd ${software_path}"/fastdfs-nginx-module-1.20/src"
sed -i "/^.*ngx_module_incs=/c\ngx_module_incs=\"/usr/include/fastdfs /usr/include/fastcommon/\"" config
sed -i "/^.*CORE_INCS=/c\CORE_INCS=\"\$CORE_INCS /usr/include/fastdfs /usr/include/fastcommon/\"" config


# 配置mod_fastdfs.conf配置
cd /etc/fdfs
# 配置base_path
sed -i "/^base_path=/c\base_path=$storage_data_path" mod_fastdfs.conf
# 配置tracker地址
sed -i "/^tracker_server=/c\tracker_server=$local_ip:22122" mod_fastdfs.conf
# 配置组名
sed -i "/^group_name=/c\group_name=$group_name" mod_fastdfs.conf
# 配置store_path0
sed -i "/^store_path0=/c\store_path0=$storage_data_path" mod_fastdfs.conf
# 配置url是否包含group
sed -i "/^url_have_group_name=/c\url_have_group_name=true" mod_fastdfs.conf

# storage nginx安装
cd ${software_path}"/nginx-1.17.1"
# configure
./configure --prefix=${storage_nginx_path} --add-module=${software_path}"/fastdfs-nginx-module-1.20/src"
# 编译安装
make || exit 1
make install || exit 1
# storage nginx配置
cd ${storage_nginx_path}"/conf"
# storage nginx端口号设置
sed -i "/^\s*\r*\listen.*$/c\listen  8080;" nginx.conf
# storage nginx ip 地址设置
sed -i "/^\s*\r*server_name.*$/c\server_name  $local_ip;" nginx.conf
# 添加location节点
sed -i "/^.*\#error_page.*$/i\        location \/$group_name\/M00 {" nginx.conf
sed -i "/^.*\#error_page.*$/i\            root $storage_data_path\/data;" nginx.conf
sed -i "/^.*\#error_page.*$/i\            ngx_fastdfs_module;" nginx.conf
sed -i "/^.*\#error_page.*$/i\        }" nginx.conf
# 添加软链接
ln -s  ${storage_data_path}"/data"  ${storage_data_path}"/data/M00"

# tracker nginx安装
cd ${software_path}"/nginx-1.17.1"
# configure
./configure --prefix=${tracker_nginx_path} --add-module=${software_path}"/fastdfs-nginx-module-1.20/src"
make || exit 1
make install || exit 1
# tracker nginx配置
cd ${tracker_nginx_path}"/conf"
# tracker nginx端口号设置
sed -i "/^\s*\r*listen.*$/c\listen  8081;" nginx.conf
# tracker nginx ip 地址设置
sed -i "/^\s*\r*server_name.*$/c\server_name  $local_ip;" nginx.conf

# 配置upstram
sed -i "/^.*\#gzip.*$/i\        upstream fdfs_$group_name {" nginx.conf
sed -i "/^.*\#gzip.*$/i\            server ${local_ip}:8080;" nginx.conf
sed -i "/^.*\#gzip.*$/i\        }" nginx.conf
# 添加location节点
sed -i "/^.*\#error_page.*$/i\        location \/$group_name\/M00 {" nginx.conf
sed -i "/^.*\#error_page.*$/i\            proxy_pass http://fdfs_$group_name;" nginx.conf
sed -i "/^.*\#error_page.*$/i\        }" nginx.conf

# 防火墙
firewall-cmd --zone=public --add-port=22122/tcp --permanent
firewall-cmd --zone=public --add-port=23000/tcp --permanent
firewall-cmd --zone=public --add-port=8080/tcp --permanent
firewall-cmd --zone=public --add-port=8081/tcp --permanent

# 运行
fdfs_trackerd /etc/fdfs/tracker.conf
fdfs_storaged /etc/fdfs/storage.conf
${tracker_nginx_path}"/sbin/nginx"
${storage_nginx_path}"/sbin/nginx"


echo -e "[--------------------fastdfs install finish--------------------]"
