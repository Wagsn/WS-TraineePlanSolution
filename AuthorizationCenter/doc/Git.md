# 本项目使用到的Git命令

## 仓用命令

### 初始化仓库

```bash
git init
```
### 查看仓库状态

```bash
git status
```

### 推送到远程主分支

```bash
git push origin master
```

### 切换到其它分支

```bash
git checkout <branch-name>
```

## 跟踪远程

## 拉取

- 从GitHub
git pull Github-WS-TraineePlanSolution master

- 从Tuleap
git pull XKJ-WS-TraineePlanSolution master

## 推送

- 到GitHub

git push Github-WS-TraineePlanSolution master

- 到Tuleap

## 分支管理


### 查看分支

```bash
git branch -v
```

### 创建分支

```bash
git branch <branch-name> # 创建分支
```

### 切换分支

```bash
git checkout <branch-name> # 切换分支
```

## 同步

### 拉取

```bash
git pull <repository-name> <branch-name>
```

### 推送

```bash
git push <repository-name> <branch-name>
```

## 仓库管理

### 仓库初始化

```bash
git init
```

### 查看仓库状态

```bash
git status
```

## 标签

每次发版后，需要在master分支上打上版本标签，方便以后获取固定版本代码，之前都没打，后面吧0.5 0.6也补上。
添加本地标签： git tag -a v0.7 -m 'v0.7版本'
提交标签到远端：git push origin --tags

补充0.5. 0.6找到 当前最后一次提交的 commit id
git tag -a v0.5 [commit id] -m 'v0.5'

## 问题及解决方案

### 凭据问题

描述：
在提取提交时提示权限不足，权限用户不是你（主要在于这个电脑可能之前被其他人使用过但是配置没有被清理）

原因：
使用了之前别人用过Git的电脑

步骤：
1. 打开 控制面板\用户帐户\凭据管理器（或者直接在搜索栏搜索凭据）
2. 在 Windows凭据\普通凭据 中找到你要推送的远程地址，修改用户名和密码
