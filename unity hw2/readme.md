博客地址https://blog.csdn.net/DDghsot/article/details/79879711

# unity 3d hw2 简答题
## 游戏对象运动的本质是什么？
#### 答：
> * 运动的本质就是游戏对象在每一帧进行位置的变换。
#### 请用三种方法以上方法，实现物体的抛物线运动。（如，修改Transform属性，使用向量Vector3的方法…）

> 由于抛物线可以看做水平速度为V，竖直方向速度为gt的运动


> 抛物线的速度公式也可以表示为 $  v_0 = v_0 + \frac{1}{2}gt^2 $ 

> * 根据公式，构造一个Vector3的速度，直接给transform赋值
```
public class 抛物线 : MonoBehaviour {
	private Vector3 speed = Vector3.right;
	private float gravity = 1;
	void Update () {
		speed += Vector3.down * Time.deltaTime * gravity;
		this.transform.position += speed * Time.deltaTime;
	}
}
```
> * 还可以使用translate函数
```
public class 抛物线 : MonoBehaviour {
	private Vector3 speed = Vector3.right;
	private float gravity = 1;
	void Update () {
		speed += Vector3.down * Time.deltaTime * gravity;
		this.transform.Translate ( speed * Time.deltaTime);
	}
}
```
> * 还可以分解速度向量
```
public class 抛物线 : MonoBehaviour {
	private Vector3 horizonSpeed = Vector3.right;
	private float gravity = 1;
	private Vector3 VerticalSpeed = Vector3.zero;
	void Update () {
	    VerticalSpeed += Vector3.down * Time.deltaTime * gravity;
	  	this.transform.position += VerticalSpeed * Time.deltaTime;
		this.transform.position += horizonSpeed * Time.deltaTime;
	}
}
```

# 写一个程序，实现一个完整的太阳系， 其他星球围绕太阳的转速必须不一样，且不在一个法平面上。

> * 用球体表示八大行星，月球和太阳，将它们放在x轴依次排好，大小和距离粗略的调整一下。

![此处输入图片的描述][1]

> * 然后去老师给的网站找贴图http://www.tupian114.com/3dtu/61437.html###
~~（每下一张图片居然就要扫一次公众号，一个qq号或者微信号一天只有五次下载次数，还要我换账号，绝了）~~

> * 然后将贴图放到球体上

![此处输入图片的描述][2]

> * 接下来就是写脚本，很简单，用roateAround函数就好了，注意不能在同一法平面，注意旋转的法向量x应为0，否则法平面不经过初始位置，对于自转并没有设置，因此球体公转一圈也会自转一圈。

![此处输入图片的描述][3]


> * 为了让星体旋转的时候轨道更清晰，添加一个组件显示轨迹

![此处输入图片的描述][4]


> * 对于月球的旋转，要注意一点的就是月球围绕地球旋转，那么月球应当是地球的子元素，但是这样的话月球就是相对于地球定位，即便月球不添加旋转函数，地球公转一圈后月球也会围着地球转一圈，要解决这个问题，就在地球下面添加一个空元素（大小与地球相同），然后使月球成为该空元素的子元素，可以观察到地球旋转的时候空元素是不会自转的，因此使得月球的自转不会被地球干扰。

![此处输入图片的描述][5]



# 牧师与恶魔
## 列出游戏中提及的事物（Objects）
> * 牧师
> * 恶魔
> * 河流
> * 左岸和右岸
> * 船

## 用表格列出玩家动作表（规则表），注意，动作越少越好
| 动作 | 条件 | 操作 |
| :-: | :-: | :-: |
| 开船 | 船上有人 | 点击船 |
| 上船 | 船与人在同一岸且船有空位 | 点击岸上的人 |
| 下船 | 无 | 点击船上的人 | 
| 胜利 | 左岸上有6个人物（不包括船上） |  无  |
| 失败 | 某一岸上有牧师且恶魔比牧师多 | 无 |


  [1]: https://github.com/ddghost/unity3d/blob/master/unity%20hw2/resources/%E5%A4%AA%E9%98%B3%E7%B3%BB%E6%8E%92%E5%88%97.jpg
  [2]: https://github.com/ddghost/unity3d/blob/master/unity%20hw2/resources/%E8%B4%B4%E5%9B%BE.jpg
  [3]: https://github.com/ddghost/unity3d/blob/master/unity%20hw2/resources/%E4%BB%A3%E7%A0%81.jpg
  [4]: https://github.com/ddghost/unity3d/blob/master/unity%20hw2/resources/%E8%BD%A8%E8%BF%B9.jpg
  [5]: https://github.com/ddghost/unity3d/blob/master/unity%20hw2/resources/%E7%BB%93%E6%9E%84.jpg
