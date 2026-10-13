dnSpy.Console.dll [选项] [文件或路径1] [文件或路径2] [...]

  --sdk-project   generate SDK style project files
  --asm-path 路径   程序集搜索路径。路径可以用 ';' 分隔。亦可使用多个 --asm-path 来指定多个路径。
  --user-gac 路径   用户 GAC 路径。路径可以用 ';' 分隔或你可以使用 --user-gac's 选择
  --no-gac       不要从 GAC 中搜寻程序集。可与 --no-stdlib 参数搭配使用
  --no-stdlib    不要引用 mscorlib 库
  --no-sln       不要创建 .sln 解决方案文件
  --sln-name 名称   .sln 解决方案文件的名称
  --threads N    工作线程的数目。默认每个CPU核心使用一个线程。
  --no-resources   不要解包资源文件
  --no-resx      不要创建 .resx 资源文件
  --no-baml      不要将 BAML 反编译为 XAML
  --no-color     不要高亮文本
  --spaces N     缩进大小 (单位为空格)，指定 0 时则使用一个制表符
  --vs N         Visual Studio 版本，2005，2008，...，2017
  --project-guid N   项目的 GUID
  -t 名称          反编译指定名称的类型到标准输出流。指定名称的格式为 [命名空间.名称]  或 [名称]，不区分大小写
  --type 名称      与 -t 相同
  --md N         反编译元数据标记 N 的成员到输出窗口
  --gac-file 程序集   反编译 GAC 中的程序集。可使用完整的程序集名称来确定要使用的程序集版本。
  -r             递归搜索 .NET 文件来反编译
  -o 输出目录        输出目录: 
  -l 语言          设置语言，默认为 C#。可使用 Guid。

语言: 
  C# ({4162dada-67c3-4de4-a5f3-6552c8353ece})
  Visual Basic ({bba40092-76b2-4184-8e81-0f1e3ed14e72})
  IL ({a4f35508-691f-4bd0-b74d-d5d5d1d0e8e6})
  IL with C# ({ca52a515-12ae-4182-bc88-81ed037c3d32})

语言选项：
所有布尔选项可以使用 'no-' 或 'dont-' 显示。如 --dont-sort-members

  C# ({4162dada-67c3-4de4-a5f3-6552c8353ece})
  Visual Basic ({bba40092-76b2-4184-8e81-0f1e3ed14e72})
    --member-order	(String = mpeft) 反编译顺序
    --anon-methods	(Boolean = True) 反编译 Lambda 表达式和匿名函数
    --expr-trees	(Boolean = True) 反编译表达式树 (Expression Trees)
    --yield	(Boolean = True) 反编译枚举器 (yield return)
    --async	(Boolean = True) 反编译异步方法 (async/await)
    --auto-props	(Boolean = True) 反编译自动生成的属性
    --auto-events	(Boolean = True) 反编译自动生成的事件
    --using-stmt	(Boolean = True) 反编译 using 语句
    --foreach-stmt	(Boolean = True) 反编译 foreach 语句
    --lock-stmt	(Boolean = True) 反编译 lock 语句
    --switch-string	(Boolean = True) 反编译 string 类型的 switch 语句
    --using-decl	(Boolean = True) 添加 using 声明
    --query-expr	(Boolean = True) 反编译查询表达式
    --ambig-full-names	(Boolean = True) 将命名空间名添加到具有相同名称的类型
    --full-names	(Boolean = False) 将命名空间名添加到所有类型
    --use-debug-syms	(Boolean = True) 如果可用，使用由调试符号所声明的变量名称
    --obj-inits	(Boolean = True) 反编译对象或集合的初始值设定项
    --xml-doc	(Boolean = True) 在反编译的代码中显示 XML 文档
    --remove-emtpy-ctors	(Boolean = True) 移除空的默认构造函数
    --inc-dec	(Boolean = True) 使用增量和减量运算符
    --make-assign-expr	(Boolean = True) 在流程控制语句中使用赋值表达式 (如 while ((count = Do()) != 0) ;)
    --always-create-ex-var	(Boolean = False) 在 catch 块中总是生成异常变量，除非类型为 Object
    --tokens	(Boolean = True) 显示元数据标记、相对虚拟地址和文件偏移
    --sort-members	(Boolean = False) 排序方法、字段、属性、事件和类型
    --show-all	(Boolean = False) 显示编译器生成的隐藏类型和方法
    --system-first	(Boolean = True) 对 using 排序时将 "System" 指令排在首位
    --max-array-elems	(Int32 = 10000) 要显示的数组元素最大数量
    --max-string-length	(Int32 = 20000) 最大字符串长度
    --sort-custom-attrs	(Boolean = False) 排序自定义特性
    --src-order	(Boolean = True) 按源代码顺序排序成员
    --field-initializers	(Boolean = True) 允许在字段声明时初始化值
    --one-ca-per-line	(Boolean = True) 每个自定义特性独占一行
    --internal-modifier-type	(Boolean = True) 为类型添加 internal 关键字
    --private-modifier-member	(Boolean = True) 为类型成员添加 private 关键字
    --remove-new-delegate-class	(Boolean = True) 将 "new delegate-class(xxx)" 替换为 "xxx"
    --hex	(Boolean = False) 以十六进制显示数字
    --emit-calli-as-invoke	(Boolean = False) 反编译 calli 指令为调用表达式
    --insert-parentheses-for-readability	(Boolean = True) 插入括号以增强可读性

  IL ({a4f35508-691f-4bd0-b74d-d5d5d1d0e8e6})
  IL with C# ({ca52a515-12ae-4182-bc88-81ed037c3d32})
    --comments	(Boolean = False) 显示 IL 操作码注释
    --xml-doc	(Boolean = True) 在反编译的代码中显示 XML 文档
    --tokens	(Boolean = True) 显示元数据标记、相对虚拟地址和文件偏移
    --bytes	(Boolean = True) 显示 IL 指令字节
    --sort-members	(Boolean = False) 排序方法、字段、属性、事件和类型
    --pdb-info	(Boolean = True) 显示行号和文件名 (如果可用)
    --max-string-length	(Int32 = 20000) 最大字符串长度
    --hex	(Boolean = False) 以十六进制显示数字

例如: 
  dnSpy.Console.dll -o C:\out\path C:\some\path
      反编译目录中所有 .NET 文件并保存到 C:\out\path
  dnSpy.Console.dll -o C:\out\path -r C:\some\path
      反编译以上目录及子目录中所有 .NET  文件
  dnSpy.Console.dll -o C:\out\path C:\some\path\*.dll
      反编译目录中所有 .NET 的 dll 文件并保存到 C:\out\path
  dnSpy.Console.dll --md 0x06000123 file.dll
      反编译标记为 0x06000123 的成员
  dnSpy.Console.dll -t system.int32 --gac-file "mscorlib, Version=4.0.0.0"
      从 mscorlib 反编译 System.Int32

错误: 未找到类型 Verse.TranslateExtensions
