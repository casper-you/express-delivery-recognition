------

## 概要

~~~md
# Express Delivery Identification in HALCON

一个基于 **C# WinForms + HALCON** 开发的快递面单识别上位机项目。  
该项目支持从本地文件夹批量加载图片，调用 HALCON 完成目标检测与 OCR 识别，并通过串口将识别结果发送给下位机，也支持接收下位机命令实现自动识别与自动切换图片。

## 项目特点

- 基于 **Visual Studio Community 2022**
- 使用 **C# WinForms** 开发图形界面
- 使用 **HALCON** 进行图像处理与 OCR 识别
- 支持选择本地文件夹批量加载图片
- 支持显示当前图片与识别结果
- 支持串口通信
- 支持日志显示与本地日志文件保存
- 支持下位机串口命令控制：
  - `recognition`：识别当前图片
  - `next`：切换到下一张图片

---

## 功能说明

### 1. 图片加载
- 通过界面选择包含测试图片的文件夹
- 自动读取文件夹内的 `jpg / png / bmp / tif` 图片
- 在界面中显示当前图片
- 支持切换到下一张图片

### 2. 图像识别
- 对当前图片进行预处理
- 通过 Shape Model 匹配定位目标区域
- 基于 OCR 字典进行文字识别
- 在界面日志中输出识别结果
- 保存最近一次识别结果

### 3. 串口通信
- 支持刷新串口列表
- 支持打开/关闭串口
- 支持手动发送消息
- 支持接收串口消息并显示在接收框中
- 接收命令后自动执行对应操作

### 4. 串口命令控制
下位机可通过串口发送以下命令：

#### `recognition`
识别当前图片，并返回识别结果：
`next`
切换下一张图片：
~~~

返回示例：

```text
RESULT:XXXXXXXX
```

### `next`

切换到下一张图片，并返回确认消息：

```text
next
```

返回示例：

```text
OK:NEXT
```

###  日志功能

- 所有操作会显示在界面日志框中
- 同时自动写入本地日志文件：

```text
Recognition_Log.txt
```

------

## 开发环境

- Visual Studio Community 2022
- .NET Framework
- HALCON
- Windows

------

## 项目结构说明

建议仓库结构如下：

```text
项目根目录
├─ C#上位机
├─ HALCAN图像处理
│  └─ main.hdev
├─ 模板文件
├─ 数据集
├─预处理ROI区域
├─vspd6.9
└─ README.md
```

------

## 使用前准备

### 1. 安装 HALCON

请确保本机已正确安装 HALCON，并且项目已经正确引用：

- `HalconDotNet`

### 2. 配置模型文件

当前代码中模板路径为硬编码路径，需要你根据实际情况修改：

```csharp
string modelPath3 = @"C:/Users/1/Desktop/Express Delivery Recognition  in halcan/模板文件/ShipTo_Model3.shm";
string modelPathFed = @"C:/Users/1/Desktop/Express Delivery Recognition  in halcan/模板文件/ShipTo_Model_fed.shm";
```

建议改成相对路径，例如：

```csharp
string modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models", "ShipTo_Model3.shm");
```

### 3. 配置 OCR 字典文件

当前代码中 OCR 字典加载方式为：

```csharp
HOperatorSet.ReadOcrClassMlp("Document_0-9A-Z_NoRej.omc", out hv_OCRHandle);
```

建议将该文件放到程序运行目录，或者改为明确的相对路径。

------

## 使用方法

### 1. 打开程序

运行程序后，系统会自动：

- 初始化 HALCON 模型
- 初始化串口模块
- 初始化串口界面

### 2. 选择图片文件夹

点击“选择文件夹”按钮，选择包含待识别图片的文件夹。
程序会自动加载图片列表，并显示第一张图片。

### 3. 手动识别

点击“识别当前图片”按钮后，程序将：

- 对当前图片进行识别
- 在日志中输出识别结果
- 如果串口已打开，则自动发送：

```text
RESULT:识别结果
```

### 4. 切换图片

点击“下一张”按钮后，程序将切换到下一张图片。
当切换到最后一张后，会自动回到第一张。

### 5. 打开串口

在串口区域中：

- 选择串口号
- 选择波特率
- 点击“打开串口”

### 6. 手动发送

在发送框输入内容，点击“发送”按钮即可通过串口发送数据。

### 7. 下位机自动控制

下位机可发送命令：

```text
recognition
```

识别当前图片，并回传结果。

```text
next
```

切换到下一张图片，并回传确认消息。

------

## vspd6.9文件

```
Express Delivery Recognition  in halcan\vspd6.9\vspd6.9\Cracked
```

路径下的vspdconfig.exe为虚拟串口程序

## 通信协议

### 上位机接收命令

| 命令          | 说明             |
| ------------- | ---------------- |
| `recognition` | 识别当前图片     |
| `next`        | 切换到下一张图片 |

### 上位机发送数据

| 返回内容          | 说明           |
| ----------------- | -------------- |
| `RESULT:xxx`      | 返回识别结果   |
| `OK:NEXT`         | 已切换到下一张 |
| `ERROR:NO_RESULT` | 本次识别无结果 |

### 分包方式

当前串口通信按 `\r\n` 作为一条完整消息的结束标志。
因此串口助手或下位机发送消息时，建议追加回车换行。

------

## 识别流程概述

项目中的识别流程大致如下：

1. 读取当前图片
2. 中值滤波、腐蚀、差分增强
3. 阈值分割与形态学处理
4. 提取 ROI 区域
5. 进行 Shape Model 匹配
6. 根据匹配位置推算 OCR 区域
7. 对 OCR 区域进行图像校正与文字分割
8. 调用 OCR 字典识别字符
9. 输出识别结果
10. 通过串口发送识别结果

------

## 注意事项

### 1. 模型路径问题

当前代码中部分模型文件使用了固定绝对路径。。

### 2. HALCON 版本兼容

不同 HALCON 版本在模型文件、OCR 文件、算子行为上可能存在差异。
请尽量注明你开发和测试所使用的 HALCON 版本。

### 3. OCR 字典文件

`Document_0-9A-Z_NoRej.omc` 需要随项目一起提供，或者在 README 中说明获取方式。

### 4. 串口命令格式

当前命令解析采用小写字符串匹配，并以 `\r\n` 分包。
若下位机协议不同，需要同步修改。

### 5. 日志文件

日志默认保存到程序运行目录下的：

```text
Express Delivery Recognition  in halcan\C#上位机\Express Delivery Identification in halcan\Express Delivery Identification in halcan\bin\Debug\Recognition_Log.txt
```

------

## 已实现功能

-  文件夹加载图片
-  HALCON 图像显示
-  Shape Model 匹配
-  OCR 识别
-  串口打开/关闭
-  串口发送/接收
-  下位机命令控制识别
-  自动切换下一张图片
-  日志显示与日志文件保存

------

## 后续可优化方向

-  模型路径改为相对路径
-  增加配置文件支持
-  增加识别结果专用显示区域
-  增加批量自动识别模式
-  增加识别结果导出功能
-  增加异常图片标记功能
-  优化 UI 布局与交互体验

------

## 开源说明

欢迎交流、学习和二次开发。
如果你在使用过程中发现问题，欢迎提交 email:2633587880@qq.com

------

## 作者

本项目由作者基于 **C# + HALCON + WinForms** 开发，用于实现快递面单图像识别与上下位机串口通信。