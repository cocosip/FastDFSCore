#!/bin/bash
# sed -i 's/\r$//' install.sh
# 默认参数,安装模式,T(Tracker),S(Storage),A(All)
install_mode="all"
# 软件所在目录
software_path="/usr/local/fastdfs_download"
# tracker运行目录
tracker_base_path="/usr/local/fastdfs/tracker"
# tracker nginx目录
tracker_nginx_path="/usr/local/nginx/tracker_nginx"
# tracker 端口号
tracker_port=22122
# tracker nginx端口
tracker_nginx_port=8081
# tracker 对应的storage ip地址
tracker_storage_ip=`/sbin/ifconfig -a|grep inet|grep -v 127.0.0.1|grep -v inet6|awk '{print $2}'|tr -d "addr:"`
# tracker 对应的storage nginx端口号
tracker_storage_nginx_port=8080

# storage 配置的tracker ip
storage_tracker_ip=`/sbin/ifconfig -a|grep inet|grep -v 127.0.0.1|grep -v inet6|awk '{print $2}'|tr -d "addr:"`
# storage 配置的tracker 端口
storage_tracker_port=22122
# storage运行目录
storage_base_path="/usr/local/fastdfs/storage"
# storage nginx目录
storage_nginx_path="/usr/local/nginx/storage_nginx"
# storage文件保存的路径
storage_data_path="/usr/local/fastdfs/storage_data"
# storage端口号
storage_port=23000
# storage nginx端口号
storage_nginx_port=8080

# 当前组
group_name="group1";
# 客户端运行目录
client_base_path="/usr/local/fastdfs/client";
# 当前ip
local_ip=`/sbin/ifconfig -a|grep inet|grep -v 127.0.0.1|grep -v inet6|awk '{print $2}'|tr -d "addr:"`



# 判断命令是否存在
command_exists() {
	command -v "$@" >/dev/null 2>&1
}
# 获取服务器的IP地址
get_server_ip() {
	local server_ip=""
	local interface_info=""

	if command_exists ip; then
		interface_info="$(ip addr)"
	elif command_exists ifconfig; then
		interface_info="$(ifconfig)"
	fi

	server_ip=$(echo "$interface_info" | \
		grep -oE "[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}" | \
		grep -vE "^192\.168|^172\.1[6-9]\.|^172\.2[0-9]\.|^172\.3[0-2]\.|^10\.|^127\.|^255\.|^0\." | \
		head -n 1)

	# 自动获取失败时，通过网站提供的 API 获取外网地址
	if [ -z "$server_ip" ]; then
		 server_ip="$(wget -qO- --no-check-certificate https://ipv4.icanhazip.com)"
	fi
	echo "$server_ip"
}
# 获取第一个字符
first_character() {
	if [ -n "$1" ]; then
		echo "$1" | cut -c1
	fi
}
# 判断输入内容是否为数字
is_number() {
	expr "$1" + 1 >/dev/null 2>&1
}

# 检测是否使用root权限运行
check_root() {
	local user="$(id -un 2>/dev/null || true)"
	if [ "$user" != "root" ]; then
		cat >&2 <<-'EOF'
		权限错误, 请使用 root 用户运行此脚本!
		EOF
		exit 1
	fi
}
# tracker配置
set_global_config() {
    local input=""
    # 设置安装模式
    [ -z "$install_mode" ]
    local install_mode_list="tracker storage all"
    local i=0
    cat >&1 <<-'EOF'
	请输入 fastdfs 安装模式(tracker|storage|all)
	tracker:只安装Tracker
    storage:只安装Storage
    all:全部都安装
	EOF
    while true
	do
		for c in $install_mode_list; do
			i=$(expr $i + 1)
			echo "(${i}) ${c}"
		done

		read -p "(默认: ${install_mode}) 请选择 [1~$i]: " input
		if [ -n "$input" ]; then
			if is_number "$input" && [ $input -ge 1 ] && [ $input -le $i ]; then
				install_mode=$(echo "$install_mode_list" | cut -d' ' -f ${input})
			else
				echo "请输入有效数字 1~$i!"
				i=0
				continue
			fi
		fi
		break
	done
    input=""
	i=0
    cat >&1 <<-EOF
	-----------------------------
	安装模式 = ${install_mode}
	-----------------------------
	EOF
    # ip 地址
	[ -z "$local_ip" ]
	cat >&1 <<-'EOF'
	请输入ip地址
	可以输入主机名称、IPv4 地址或者 IPv6 地址
	EOF
	read -p "(默认: ${local_ip}): " input
	if [ -n "$input" ]; then
		local_ip="$input"
	fi

	input=""
	cat >&1 <<-EOF
	---------------------------
	ip地址 = ${local_ip}
	---------------------------
	EOF
	# 软件下载目录
	[ -z "$software_path" ]
	cat >&1 <<-'EOF'
	请输入软件下载保存的目录
	EOF
	read -p "(默认: ${software_path}): " input
	if [ -n "$input" ]; then
		software_path="$input"
	fi

	input=""
	cat >&1 <<-EOF
	---------------------------
	软件下载保存目录 = ${software_path}
	---------------------------
	EOF

	# 组名
	[ -z "$group_name" ]
	cat >&1 <<-'EOF'
	请输入组名,如 group1
	EOF
	read -p "(默认: ${group_name}): " input
	if [ -n "$input" ]; then
		group_name="$input"
	fi

	input=""
	cat >&1 <<-EOF
	---------------------------
	软件下载保存目录 = ${group_name}
	---------------------------
	EOF


}

# 安装确认
config_confirm() {
	cat >&1 <<-EOF
	请确认您的安装配置!
	-----------------------------
	安装模式 = ${install_mode}
	本地ip地址 = ${local_ip}
	软件下载保存目录 = ${software_path}
	组名 =${group_name}
	tracker运行目录 = ${tracker_base_path}
	tracker nginx模块运行目录= ${tracker_nginx_path}
	tracker端口号 = ${tracker_port}
	tracker中nginx端口 = ${tracker_nginx_port}
	tracker中对应的storage的ip地址 = ${tracker_storage_ip}
	tracker中对应的storage的nginx端口号 = ${tracker_storage_nginx_port}

	storage运行目录 = ${storage_base_path}
	storage nginx模块运行目录 = ${storage_nginx_path}
	storage 文件数据保存目录 = ${storage_data_path}
	storage 端口号 = ${storage_port}
	storage nginx模端口号 = ${storage_nginx_port}
	storage配置的tracker的ip地址 = ${storage_tracker_ip}
	storage配置的tracker的端口号 = ${storage_tracker_port}

	-----------------------------
	EOF

	cat >&1 <<-'EOF'
	是否确认开始安装?
	EOF

	read -p "(默认: y,安装) 请选择 [y/n]: " yn
	if [ -n "$yn" ]; then
		case "$(first_character "$yn")" in
			y|Y)
			
			cat >&1 <<-'EOF'			
			将为您开始安装 fastdfs
			EOF
				;;
			n|N|*)
				echo "您取消了配置,将退出安装。"
				exit 0
				;;
		esac
	fi

}

# 安装全局
install_global() {
	# 安装必要软件
	yum -y install zlib zlib-devel pcre pcre-devel gcc gcc-c++ openssl openssl-devel libevent libevent-devel perl unzip net-tools wget
	
	# 软件下载目录
	if [ ! -d ${software_path} ] ;then
	mkdir -p ${software_path}
    fi
	
	# 下载软件
	wget -P ${software_path} https://github.com/happyfish100/libfastcommon/archive/V1.0.41.zip
    wget -P ${software_path} https://github.com/happyfish100/fastdfs/archive/V6.03.tar.gz
    wget -P ${software_path} http://nginx.org/download/nginx-1.17.1.tar.gz
    wget -P ${software_path} https://github.com/happyfish100/fastdfs-nginx-module/archive/V1.21.tar.gz

	cd ${software_path}
    # 解压软件
    unzip V1.0.41.zip
    tar zxvf V6.03.tar.gz
    tar zxvf nginx-1.17.1.tar.gz
    tar zxvf V1.21.tar.gz

	# 安装libfastcommon
    cd libfastcommon-1.0.41
    ./make.sh || exit 1
    ./make.sh install || exit 1
	# 创建软链接
    ln -s /usr/lib64/libfastcommon.so /usr/local/lib/libfastcommon.so
    ln -s /usr/lib64/libfastcommon.so /usr/lib/libfastcommon.so
    ln -s /usr/lib64/libfdfsclient.so /usr/local/lib/libfdfsclient.so
    ln -s /usr/lib64/libfdfsclient.so /usr/lib/libfdfsclient.so

	# 安装fastdfs
    cd ..
    cd fastdfs-6.03
    ./make.sh || exit 1
    ./make.sh install || exit 1

	cd /etc/fdfs
    # 拷贝配置文件
    mv client.conf.sample client.conf
    mv storage.conf.sample storage.conf
    mv tracker.conf.sample tracker.conf
    # http.conf和mime.types,mod_fastdfs.conf,到/etc/fdfs下
    cp ${software_path}"/fastdfs-6.03/conf/http.conf" /etc/fdfs/
    cp ${software_path}"/fastdfs-6.03/conf/mime.types" /etc/fdfs/
    cp ${software_path}"/fastdfs-nginx-module-1.21/src/mod_fastdfs.conf" /etc/fdfs/

	# fastdfs-nginx-module配置
    cd ${software_path}"/fastdfs-nginx-module-1.21/src"
    sed -i "/^.*ngx_module_incs=/c\ngx_module_incs=\"/usr/include/fastdfs /usr/include/fastcommon/\"" config
    sed -i "/^.*CORE_INCS=/c\CORE_INCS=\"\$CORE_INCS /usr/include/fastdfs /usr/include/fastcommon/\"" config

}

# tracker配置
set_tracker_config() {
	# 端口格式检测
	is_port() {
		local port="$1"
		is_number "$port" && \
			[ $port -ge 1 ] && [ $port -le 65535 ]
	}
	# 端口检测
	port_using() {
		local port="$1"

		if command_exists netstat; then
			( netstat -ntul | grep -qE "[0-9:*]:${port}\s" )
		elif command_exists ss; then
			( ss -ntul | grep -qE "[0-9:*]:${port}\s" )
		else
			return 0
		fi

		return $?
	}

    case $install_mode in 
	    tracker | all )
	local input=""
    # tracker 运行目录
	[ -z "$tracker_base_path" ]
	cat >&1 <<-'EOF'
	请输入tracker运行目录
	EOF
	read -p "(默认: ${tracker_base_path}): " input
	if [ -n "$input" ]; then
		tracker_base_path="$input"
	fi
	input=""
	cat >&1 <<-EOF
	---------------------------
	tracker运行目录 = ${tracker_base_path}
	---------------------------
	EOF

	[ -z "$tracker_nginx_path" ]
	cat >&1 <<-'EOF'
	请输入tracker nginx 模块运行目录
	EOF
	read -p "(默认: ${tracker_nginx_path}): " input
	if [ -n "$input" ]; then
		tracker_nginx_path="$input"
	fi
	input=""
	cat >&1 <<-EOF
	---------------------------
	tracker nginx模块运行目录 = ${tracker_nginx_path}
	---------------------------
	EOF

	# tracker端口号
	[ -z "$tracker_port" ]
	while true
	do
		cat >&1 <<-'EOF'
		请输入 tracker 运行端口号
		EOF
		read -p "(默认: ${tracker_port}): " input
		if [ -n "$input" ]; then
			if is_port "$input"; then
				tracker_port="$input"
			else
				echo "输入有误, 请输入 1~65535 之间的数字!"
				continue
			fi
		fi

		if port_using "$tracker_port" ; then
			echo "端口已被占用, 请重新输入!"
			continue
		fi
		break
	done
	input=""
	cat >&1 <<-EOF
	---------------------------
	tracker端口号 = ${tracker_port}
	---------------------------
	EOF

	# tracker nginx端口号
	[ -z "$tracker_nginx_port" ]
	while true
	do
		cat >&1 <<-'EOF'
		请输入 tracker nginx端口
		EOF
		read -p "(默认: ${tracker_nginx_port}): " input
		if [ -n "$input" ]; then
			if is_port "$input"; then
				tracker_nginx_port="$input"
			else
				echo "输入有误, 请输入 1~65535 之间的数字!"
				continue
			fi
		fi

		if port_using "$tracker_nginx_port" ; then
			echo "端口已被占用, 请重新输入!"
			continue
		fi
		break
	done
	input=""
	cat >&1 <<-EOF
	---------------------------
	tracker nginx端口号 = ${tracker_nginx_port}
	---------------------------
	EOF

	# tracker对应的storage地址
	[ -z "$tracker_storage_ip" ]
	cat >&1 <<-'EOF'
	请输入 tracker 对应映射的storage的ip地址
	可以输入主机名称、IPv4 地址或者 IPv6 地址
	EOF
	read -p "(默认: ${tracker_storage_ip}): " input
	if [ -n "$input" ]; then
		tracker_storage_ip="$input"
	fi

	input=""
	cat >&1 <<-EOF
	---------------------------
	tracker对应storage的ip地址 = ${tracker_storage_ip}
	---------------------------
	EOF

	# tracker对应storage的nginx端口号
	[ -z "$tracker_storage_nginx_port" ]
	while true
	do
		cat >&1 <<-'EOF'
		请输入 tracker对应storage的nginx端口
		EOF
		read -p "(默认: ${tracker_storage_nginx_port}): " input
		if [ -n "$input" ]; then
			if is_port "$input"; then
				tracker_storage_nginx_port="$input"
			else
				echo "输入有误, 请输入 1~65535 之间的数字!"
				continue
			fi
		fi

		if port_using "$tracker_storage_nginx_port" ; then
			echo "端口已被占用, 请重新输入!"
			continue
		fi
		break
	done
	input=""
	cat >&1 <<-EOF
	---------------------------
	tracker对应storage的nginx端口号 = ${tracker_storage_nginx_port}
	---------------------------
	EOF

	;;

	*)
	echo "选择的模式${install_mode},将不配置tracker"

	esac
}


# tracker安装
install_tracker() {
	case $install_mode in 
        tracker | all )
	# 创建tracker目录
    if [ ! -d ${tracker_base_path} ] ;then
	    mkdir -p ${tracker_base_path}
    fi
    # 创建tracker nginx目录
    if [ ! -d ${tracker_nginx_path} ] ;then
	    mkdir -p ${tracker_nginx_path}
    fi
	# tracker配置
	cd /etc/fdfs
	# 配置ip地址
    sed -i "/^bind_addr=/c\bind_addr=$local_ip" tracker.conf
	# 配置tracker运行根目录
    sed -i "/^base_path=/c\base_path=$tracker_base_path" tracker.conf
    # 配置store group
    sed -i "/^store_group=/c\store_group=$group_name" tracker.conf

	# tracker nginx安装
	cd ${software_path}"/nginx-1.17.1"
	# configure
    ./configure --prefix=${tracker_nginx_path} --add-module=${software_path}"/fastdfs-nginx-module-1.21/src"
    make || exit 1
    make install || exit 1
	# tracker nginx配置
    cd ${tracker_nginx_path}"/conf"
    # tracker nginx端口号设置
    sed -i "/^\s*\r*listen.*$/c\listen  $tracker_nginx_port;" nginx.conf
    # tracker nginx ip 地址设置
    sed -i "/^\s*\r*server_name.*$/c\server_name  $local_ip;" nginx.conf

	# 配置upstram
    sed -i "/^.*\#gzip.*$/i\        upstream fdfs_$group_name {" nginx.conf
    sed -i "/^.*\#gzip.*$/i\            server ${tracker_storage_ip}:${tracker_storage_nginx_port};" nginx.conf
    sed -i "/^.*\#gzip.*$/i\        }" nginx.conf

	# 添加location节点
    sed -i "/^.*\#error_page.*$/i\        location \/$group_name\/M00 {" nginx.conf
    sed -i "/^.*\#error_page.*$/i\            proxy_pass http://fdfs_$group_name;" nginx.conf
    sed -i "/^.*\#error_page.*$/i\        }" nginx.conf

	;;

	*)
	echo "选择模式:${install_mode},不安装tracker"

	esac
}

# storage配置
set_storage_config() {
	# 端口格式检测
	is_port() {
		local port="$1"
		is_number "$port" && \
			[ $port -ge 1 ] && [ $port -le 65535 ]
	}
	# 端口检测
	port_using() {
		local port="$1"

		if command_exists netstat; then
			( netstat -ntul | grep -qE "[0-9:*]:${port}\s" )
		elif command_exists ss; then
			( ss -ntul | grep -qE "[0-9:*]:${port}\s" )
		else
			return 0
		fi

		return $?
	}

	case $install_mode in 
	    storage | all )
	local input=""
    # storage 运行目录
	[ -z "$storage_base_path" ]
	cat >&1 <<-'EOF'
	请输入storage运行目录
	EOF
	read -p "(默认: ${storage_base_path}): " input
	if [ -n "$input" ]; then
		storage_base_path="$input"
	fi
	input=""
	cat >&1 <<-EOF
	---------------------------
	storage_base_path运行目录 = ${storage_base_path}
	---------------------------
	EOF
    # storage nginx 运行目录
	[ -z "$storage_nginx_path" ]
	cat >&1 <<-'EOF'
	请输入storage nginx 模块运行目录
	EOF
	read -p "(默认: ${storage_nginx_path}): " input
	if [ -n "$input" ]; then
		storage_nginx_path="$input"
	fi
	input=""
	cat >&1 <<-EOF
	---------------------------
	storage nginx模块运行目录 = ${storage_nginx_path}
	---------------------------
	EOF
    # storage 数据保存目录
	[ -z "$storage_data_path" ]
	cat >&1 <<-'EOF'
	请输入storage数据保存目录
	EOF
	read -p "(默认: ${storage_data_path}): " input
	if [ -n "$input" ]; then
		storage_data_path="$input"
	fi
	input=""
	cat >&1 <<-EOF
	---------------------------
	storage数据保存运行目录 = ${storage_data_path}
	---------------------------
	EOF

	# storage端口号
	[ -z "$storage_port" ]
	while true
	do
		cat >&1 <<-'EOF'
		请输入 storage 运行端口号
		EOF
		read -p "(默认: ${storage_port}): " input
		if [ -n "$input" ]; then
			if is_port "$input"; then
				storage_port="$input"
			else
				echo "输入有误, 请输入 1~65535 之间的数字!"
				continue
			fi
		fi

		if port_using "$storage_port" ; then
			echo "端口已被占用, 请重新输入!"
			continue
		fi
		break
	done
	input=""
	cat >&1 <<-EOF
	---------------------------
	storage端口号 = ${storage_port}
	---------------------------
	EOF

	# storage nginx端口号
	[ -z "$storage_nginx_port" ]
	while true
	do
		cat >&1 <<-'EOF'
		请输入 storage nginx 端口号
		EOF
		read -p "(默认: ${storage_nginx_port}): " input
		if [ -n "$input" ]; then
			if is_port "$input"; then
				storage_nginx_port="$input"
			else
				echo "输入有误, 请输入 1~65535 之间的数字!"
				continue
			fi
		fi

		if port_using "$storage_nginx_port" ; then
			echo "端口已被占用, 请重新输入!"
			continue
		fi
		break
	done
	input=""
	cat >&1 <<-EOF
	---------------------------
	storage nginx端口号 = ${storage_nginx_port}
	---------------------------
	EOF

	# storage对应的tracker 地址
	[ -z "$storage_tracker_ip" ]
	cat >&1 <<-'EOF'
	请输入 storage 配置的tracker的ip地址
	可以输入主机名称、IPv4 地址或者 IPv6 地址
	EOF
	read -p "(默认: ${storage_tracker_ip}): " input
	if [ -n "$input" ]; then
		storage_tracker_ip="$input"
	fi

	input=""
	cat >&1 <<-EOF
	---------------------------
	storage配置tracker的ip地址 = ${storage_tracker_ip}
	---------------------------
	EOF

	# storage配置的tracker端口号
	[ -z "$storage_tracker_port" ]
	while true
	do
		cat >&1 <<-'EOF'
		请输入 storage 配置的tracker 端口号
		EOF
		read -p "(默认: ${storage_tracker_port}): " input
		if [ -n "$input" ]; then
			if is_port "$input"; then
				storage_tracker_port="$input"
			else
				echo "输入有误, 请输入 1~65535 之间的数字!"
				continue
			fi
		fi

		if port_using "$storage_tracker_port" ; then
			echo "端口已被占用, 请重新输入!"
			continue
		fi
		break
	done
	input=""
	cat >&1 <<-EOF
	---------------------------
	storage配置tracker的端口号 = ${storage_tracker_port}
	---------------------------
	EOF

	;;

	*)
	echo "选择的模式${install_mode},将不配置storage"
	esac
}

# storage安装
install_storage() {
	case $install_mode in 
        storage | all )
	# 创建storage运行目录
    if [ ! -d ${storage_base_path} ] ;then
	    mkdir -p ${storage_base_path}
    fi
    # 创建storage nginx目录
    if [ ! -d ${storage_nginx_path} ] ;then
	    mkdir -p ${storage_nginx_path}
    fi
	# 创建storage 文件存储数据目录
	if [ ! -d ${storage_data_path} ] ;then
	    mkdir -p ${storage_data_path}
    fi
	
	# 配置storage
	cd /etc/fdfs
	# 配置组名
	sed -i "/^group_name=/c\group_name=$group_name" storage.conf
	# 配置ip地址
	sed -i "/^bind_addr=/c\bind_addr=$local_ip" storage.conf
	# 配置storage运行目录
	sed -i "/^base_path=/c\base_path=$storage_base_path" storage.conf
	# 设置磁盘的目录
	sed -i "/^store_path0=/c\store_path0=$storage_data_path" storage.conf
	# 配置tracker ip地址
    sed -i "/^tracker_server=/c\tracker_server=$storage_tracker_ip:$storage_tracker_port" storage.conf

	# 配置mod_fastdfs.conf配置
    # 配置base_path
    sed -i "/^base_path/c\base_path=$storage_data_path" mod_fastdfs.conf
    # 配置tracker地址
    sed -i "/^tracker_server/c\tracker_server=$storage_tracker_ip:$storage_tracker_port" mod_fastdfs.conf
    # 配置组名
    sed -i "/^group_name/c\group_name=$group_name" mod_fastdfs.conf
    # 配置store_path0
    sed -i "/^store_path0/c\store_path0=$storage_data_path" mod_fastdfs.conf
    # 配置url是否包含group
    sed -i "/^url_have_group_name/c\url_have_group_name=true" mod_fastdfs.conf

	# storage nginx安装
    cd ${software_path}"/nginx-1.17.1"
	# configure
    ./configure --prefix=${storage_nginx_path} --add-module=${software_path}"/fastdfs-nginx-module-1.21/src"
	# 编译安装
    make || exit 1
    make install || exit 1

	# storage nginx配置
    cd ${storage_nginx_path}"/conf"
    # storage nginx端口号设置
    sed -i "/^\s*\r*\listen.*$/c\listen  $storage_nginx_port;" nginx.conf
    # storage nginx ip 地址设置
    sed -i "/^\s*\r*server_name.*$/c\server_name  $local_ip;" nginx.conf
    # 添加location节点
    sed -i "/^.*\#error_page.*$/i\        location \/$group_name\/M00 {" nginx.conf
    sed -i "/^.*\#error_page.*$/i\            root $storage_data_path\/data;" nginx.conf
    sed -i "/^.*\#error_page.*$/i\            ngx_fastdfs_module;" nginx.conf
    sed -i "/^.*\#error_page.*$/i\        }" nginx.conf
	# 添加软链接,必须先运行storage之后才行
    #ln -s  ${storage_data_path}"/data"  ${storage_data_path}"/data/M00"

	;;

	*)
	echo "选择模式:${install_mode},不安装storage"

	esac

}

# 设置防火墙
set_firewall() {
	if command_exists firewall-cmd; then
		if ! ( firewall-cmd --state >/dev/null 2>&1 ); then
			systemctl start firewalld >/dev/null 2>&1
		fi
		if [ "$?" = "0" ]; then
			if [ -n "$tracker_port" ]; then
				firewall-cmd --zone=public --remove-port=${tracker_port}/tcp >/dev/null 2>&1
			fi
			if [ -n "$tracker_nginx_port" ]; then
				firewall-cmd --zone=public --remove-port=${tracker_nginx_port}/tcp >/dev/null 2>&1
			fi
			if [ -n "$storage_port" ]; then
				firewall-cmd --zone=public --remove-port=${storage_port}/tcp >/dev/null 2>&1
			fi
			if [ -n "$storage_nginx_port" ]; then
				firewall-cmd --zone=public --remove-port=${storage_nginx_port}/tcp >/dev/null 2>&1
			fi


			if ! firewall-cmd --quiet --zone=public --query-port=${tracker_port}/tcp; then
				firewall-cmd --quiet --permanent --zone=public --add-port=${tracker_port}/tcp
			fi
			if ! firewall-cmd --quiet --zone=public --query-port=${tracker_nginx_port}/tcp; then
				firewall-cmd --quiet --permanent --zone=public --add-port=${tracker_nginx_port}/tcp
			fi
			if ! firewall-cmd --quiet --zone=public --query-port=${storage_port}/tcp; then
				firewall-cmd --quiet --permanent --zone=public --add-port=${storage_port}/tcp
			fi
			if ! firewall-cmd --quiet --zone=public --query-port=${storage_nginx_port}/tcp; then
				firewall-cmd --quiet --permanent --zone=public --add-port=${storage_nginx_port}/tcp
			fi
			firewall-cmd --reload
		else
			cat >&1 <<-EOF
			警告: 自动添加 firewalld 规则失败
			如果有必要, 请手动添加端口 ${tracker_port},${tracker_nginx_port},${storage_port},${storage_nginx_port} 的防火墙规则:
			    firewall-cmd --permanent --zone=public --add-port=${tracker_port}/tcp
				firewall-cmd --permanent --zone=public --add-port=${tracker_nginx_port}/tcp
				firewall-cmd --permanent --zone=public --add-port=${storage_port}/tcp
				firewall-cmd --permanent --zone=public --add-port=${storage_nginx_port}/tcp
			    firewall-cmd --reload
			EOF
		fi
	elif command_exists iptables; then
		if ! ( service iptables status >/dev/null 2>&1 ); then
			service iptables start >/dev/null 2>&1
		fi

		if [ "$?" = "0" ]; then
			if [ -n "$tracker_port" ]; then
				iptables -D INPUT -p tcp --dport ${tracker_port} -j ACCEPT >/dev/null 2>&1
			fi
			if [ -n "$tracker_nginx_port" ]; then
				iptables -D INPUT -p tcp --dport ${tracker_nginx_port} -j ACCEPT >/dev/null 2>&1
			fi
			if [ -n "$storage_port" ]; then
				iptables -D INPUT -p tcp --dport ${storage_port} -j ACCEPT >/dev/null 2>&1
			fi
			if [ -n "$storage_nginx_port" ]; then
				iptables -D INPUT -p tcp --dport ${storage_nginx_port} -j ACCEPT >/dev/null 2>&1
			fi

			if ! iptables -C INPUT -p tcp --dport ${tracker_port} -j ACCEPT >/dev/null 2>&1; then
				iptables -I INPUT -p tcp --dport ${tracker_port} -j ACCEPT >/dev/null 2>&1
			fi
			if ! iptables -C INPUT -p tcp --dport ${tracker_nginx_port} -j ACCEPT >/dev/null 2>&1; then
				iptables -I INPUT -p tcp --dport ${tracker_nginx_port} -j ACCEPT >/dev/null 2>&1
			fi
			if ! iptables -C INPUT -p tcp --dport ${storage_port} -j ACCEPT >/dev/null 2>&1; then
				iptables -I INPUT -p tcp --dport ${storage_port} -j ACCEPT >/dev/null 2>&1
			fi
			if ! iptables -C INPUT -p tcp --dport ${storage_nginx_port} -j ACCEPT >/dev/null 2>&1; then
				iptables -I INPUT -p tcp --dport ${storage_nginx_port} -j ACCEPT >/dev/null 2>&1
			fi
			
			service iptables save
			service iptables restart
		else
			cat >&1 <<-EOF
			警告: 自动添加 iptables 规则失败
			如有必要, 请手动添加端口 ${tracker_port},${tracker_nginx_port},${storage_port},${storage_nginx_port} 的防火墙规则:
			    iptables -I INPUT -p tcp --dport ${tracker_port} -j ACCEPT
				iptables -I INPUT -p tcp --dport ${tracker_nginx_port} -j ACCEPT
				iptables -I INPUT -p tcp --dport ${storage_port} -j ACCEPT
				iptables -I INPUT -p tcp --dport ${storage_nginx_port} -j ACCEPT
			    service iptables save
			    service iptables restart
			EOF
		fi
	fi

}

# 运行fastdfs
run_fastdfs(){
	case $install_mode in 
        tracker)
	# 运行tracker
	fdfs_trackerd /etc/fdfs/tracker.conf
	# 运行tracker nginx
	${tracker_nginx_path}"/sbin/nginx"
	;;
	    storage)
	# 运行storage
	fdfs_storaged /etc/fdfs/storage.conf
	sleep 5s
    # 创建软链接
	ln -s  ${storage_data_path}"/data"  ${storage_data_path}"/data/M00"
	# 运行storage nginx
	${storage_nginx_path}"/sbin/nginx"
	;;
	    all)
	# 运行tracker
	fdfs_trackerd /etc/fdfs/tracker.conf
	# 运行tracker nginx
	${tracker_nginx_path}"/sbin/nginx"
	# 运行storage
	fdfs_storaged /etc/fdfs/storage.conf
	sleep 5s
    # 创建软链接
	ln -s  ${storage_data_path}"/data"  ${storage_data_path}"/data/M00"
	# 运行storage nginx
	${storage_nginx_path}"/sbin/nginx"
	;;

	*)
	echo "选择模式:${install_mode},不安装storage"

	esac

	cat >&1 <<-EOF
	fastdfs 安装成功!
	EOF


}

do_install() {
	check_root
    set_global_config
    set_tracker_config
	set_storage_config
	config_confirm
	install_global
	install_tracker
	install_storage
	set_firewall
	run_fastdfs
}

do_install