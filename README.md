# lazy_kanimal

为动画的转换提供一个有限制的便捷方式。

_注意_：该仓库分支只会维护 kanimal_lazy 项目，如果要使用 kanimal_cli 请到[源仓库](https://github.com/skairunner/kanimal-SE)获取。

## 使用

要将程序放在 scml 项目目录下，然后为 kanimal 项目的目录创建快捷方式，也放在 scml 项目目录下，需要转换程序时，双击运行即可。

程序有两种模式：

- 将快捷方式命名为 `S` 则表示 kainmal 项目是源，程序会将 kanimal 项目转换成 scml 项目并存放在当前目录。
- 将快捷方式命名为 `D` 则表示 kainmal 项目是目标，程序会将 scml 项目转换成 kanimal 项目并存放在快捷方式指向的目录。

## 限制

程序默认不使用严格模式以及骨骼去除，对应 kanimal_cli 程序就是不使用 `-S/--strict` 选项与默认使用 `-b/--debone` 选项。
