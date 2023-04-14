# PartyGame

## 距上次GIT更新功能(4/15)
1.編寫連線功能(LobbyManager：建立房間/加入房間/進入遊戲 未完成)
2.編寫開始介面功能(RoomListUI/CreateRoomUI：UI邏輯 未完成)
3.編寫組隊大廳介面功能(InRoomUI：UI邏輯 未完成)
4.編寫物件生成功能(SpawnManager：生成玩家/ObjectPoolManager：生成武器 未完成)
5.編寫資料結構(PlayerNetworkData：玩家資料/WeaponDataList_SO：武器資料)
6.編寫網路物件池功能(NetworkObjectPool/NetworkObjectPoolRoot)
7.編寫機率計算功能(AlgorithmManager：通用機率)

## 距上次GIT更新功能(4/15)
1.編寫玩家行為功能
- ThirdPersonPlayer：
    - 拾取/丟棄物品 未完成
    - 攻擊 未完成

2.編寫玩家物件功能
- SpawnManager：
    - 玩家死亡事件 未完成
    - 生成玩家 完成
    - 移除玩家 未完成

3.編寫武器物件功能
- ObjectPoolManager：
    - 生成武器(回合開始時) 完成
    - 生成武器(拾取武器時) 未完成

4.編寫獵殺任務功能
- TaskManager：
    - 新增獵殺任務(用於初始化任務清單) 完成
    - 移除獵殺任務(用於更新任務清單) 完成
    - 分配服裝 完成
    - 發布獵殺任務 完成
    - 重製獵殺任務清單 完成

5.編寫安全區功能
- SafeAreaManager：
    - 啟動安全區事件 完成
    - 關閉安全區事件 完成
    - 初始化安全區資訊 完成
    - 地圖攻擊循環 完成
- SafeArea：
    - 安全區物件邏輯 未完成

6.編寫介面UI功能
- GameManager：
    - 玩家遊玩介面初始化 未完成
- StateUI：
    - 狀態UI更新事件 血量 完成
- TaskUI：
    - 任務UI更新事件 完成
- SafeAreaUI：
    - 顯示提示事件 完成

7.編寫資料結構
- PlayerNetworkData：玩家網路資料 更新
- TaskNetworkData：任務網路資料 新增
- SafeAreaNetworkData：安全區網路資料 新增
- ClothesDataList_SO：服裝資料 新增
- TaskDataList_SO：任務資料 新增
- SafeAreaDataList_SO：安全區資料 新增
