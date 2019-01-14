# 实习计划解决方案

## 项目1 授权中心（权限管理系统）-开发中

- 用户管理（CRUD）
	- 用户列表：每项包含用户的简要信息
	- 用户新增：新增一个用户，需要填写必填数据
	- 用户详情：查看一个用户的详细数据
		- 角色列表：一个用户绑定的角色
			- 角色详情：角色的详情，等价于角色列表中的角色详情
			- 角色编辑：角色的编辑，等价于角色列表中的角色编辑
			- 删除绑定：解除该用户与该角色的绑定
	- 用户编辑：编辑用户的信息
	- 用户删除：删除该用户
- 登陆模块
	- 登陆系统：用户登陆本系统
	- 登陆保持：登陆系统之后登陆信息将会被保存在Sessin中
	- 登陆过滤：未登录页面将会重定向到登陆界面
- 角色管理（CRUD）
	- 角色列表：系统所有角色
	- 角色新增：新增一个角色
	- 角色详情：角色的详细情况
	- 角色编辑：编辑角色的信息
	- 角色删除：删除一个角色
- 角色绑定（CRUD -不易用操作，等待UI大改）
	- 用户角色列表
	- 用户角色新增
	- 用户角色详情
	- 用户角色编辑
	- 用户角色删除
- 组织管理（CRUD -登陆用户不能在组织管理界面访问到与自己无关（绑定角色没有关联的）的其它组织）
	- 组织列表
		- 树形菜单：更加形象的展示组织架构，TODO：增改删事件绑定与接口对接（需要添加新的批量操作接口）
	- 组织新增
	- 组织详情
	- 组织编辑
	- 组织删除
- 权限管理（CRUD）
	- 权限列表
	- 权限新增
	- 权限详情
	- 权限编辑
	- 权限删除
- 授权管理（CURD -角色组织权限三者关联）
	- 列表
	- 增删查改

---

## Git分支管理

master 主分支，最新发布版

development 开发分支，正在开发的代码

v1.0.0.0000 版本分支，历史版本

版本控制：没次代码提交到开发分支，当通过测试后发布到主分支，版本更替时创建版本分支