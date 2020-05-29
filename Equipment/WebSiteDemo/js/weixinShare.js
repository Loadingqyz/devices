var WeiXinShare = function(){
	
	// 需要分享的几个API接口名称
	var shareApiList = ['onMenuShareTimeline','onMenuShareAppMessage','onMenuShareQQ','onMenuShareWeibo'];
	
	// 扩展api权限，防止页面中已经引用了一些接口权限，则需要删掉之前的微信注册代码（wx.config），然后再调用这个方法进行扩展权限
	this.extendApiList = function(apiList){
		if (apiList) {
			shareApiList = shareApiList.concat(apiList);
		}
	};
	
	// 注册函数
	this.config = function(appId, timestamp, nonceStr, signature){
		wx.config({
			debug:false, // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
			appId:appId, // 必填，公众号的唯一标识
			timestamp:timestamp, // 必填，生成签名的时间戳
			nonceStr:nonceStr, // 必填，生成签名的随机串
			signature:signature,// 必填，签名，见附录1
			jsApiList:shareApiList // 必填，需要使用的JS接口列表，所有JS接口列表见附录2
		});
	};
	
	this.share = function(shareData){
		wx.ready(function(){
			
			wx.checkJsApi({
				jsApiList:shareApiList, // 需要检测的JS接口列表，所有JS接口列表见附录2,
				success: function(res){
				
					// 以键值对的形式返回，可用的api值true，不可用为false
					// 如：{"checkResult":{"chooseImage":true},"errMsg":"checkJsApi:ok"
					if (res && res.checkResult)
					{								
						checkJsApiFlag = true;								
						
						// 分享到朋友圈
						wx.onMenuShareTimeline({
							title: shareData.title, // 分享标题
							link: shareData.link, // 分享链接
							imgUrl: shareData.img_url, // 分享图标
							success: function (){},
							cancel: function (){}
						});
						
						// 分享给朋友
						wx.onMenuShareAppMessage({
							title: shareData.title, // 分享标题
							desc: shareData.desc, // 分享描述
							link: shareData.link, // 分享链接
							imgUrl: shareData.img_url, // 分享图标
							success: function () {},
							cancel: function () {}
						});		
						
						// 分享到QQ
						wx.onMenuShareQQ({
							title: shareData.title, // 分享标题
							desc: shareData.desc, // 分享描述
							link: shareData.link, // 分享链接
							imgUrl: shareData.img_url, // 分享图标
							success: function () {},
							cancel: function () {}
						});
					}
					
					// 两个小时自刷新，否则签名失效
					window.setInterval(function(){
						window.location = window.location.href;
					}, 2*3600*1000);
				},
				fail:function(){}
			});			
			
		});
	}
}