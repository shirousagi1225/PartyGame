# PartyGame

## 距上次GIT更新功能(04/29)
1.編寫玩家行為功能
- ThirdPersonPlayer：
    - 隱身 未完成
        - 新增隱身特效材質及VFX初始化方式
        - 新增隱身功能的啟動方式
        - 新增切換移動動畫狀態方式

2.編寫渲染器功能
- RendererManager：
    - 隱身特效 未完成
        - 隱身特效 
            - ==用於執行隱身特效的流程==
        - 溶解啟動 
            - ==隱身特效的啟動階段==
        - 溶解解除 
            - ==隱身特效的失效階段==
        - 熱擾動控制 
            - ==用於根據移動狀態同步改變熱擾動程度==
    - 技能運作時間 完成
        - ==技能時間計算,受攻擊會立即解除技能==
- SpawnManager：
    - 玩家硬直事件 完成
        - ==限制玩家移動,達到僵直效果==

3.編寫資料結構
- PlayerNetworkData：玩家網路資料 更新
- AnimationDataList_SO：動畫資料 新增

## 距上次GIT更新功能(04/23)
1.編寫玩家行為功能
- ThirdPersonPlayer：
    - 攻擊 未完成
        - 修正無法攻擊到主機玩家Bug

2.編寫武器物件功能
- ObjectPoolManager：
    - 生成武器(拾取武器時) 未完成
        - 拾取武器同步替換玩家所持武器模型功能 ==擱置==

3.編寫介面UI功能
- StateUI：
    - 狀態UI更新事件 武器欄 未完成
        - 拾取武器更換對應UI圖示

4.製作Shader
- Barrier：
    - 能量護盾Shader 未完成
- ForceField：
    - 能量護罩Shader 未完成
- Dissolve：
    - 溶解Shader 未完成
        - ==用於隱身特效展示==
- HeatDistortion：
    - 熱變形Shader 未完成
        - ==用於隱身特效展示==

5.製作VFX
- Dissolve_VFX：
    - 製作溶解VFX 未完成
        - ==用於隱身特效展示==
- HeatDistortion_VFX：
    - 製作熱變形VFX 未完成
        - ==用於隱身特效展示==

6.新增天空盒

7.編寫資料結構
- PlayerNetworkData：玩家網路資料 更新

## 距上次GIT更新功能(04/15)
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

## 距上次GIT更新功能(04/03)
1.編寫連線功能(LobbyManager：建立房間/加入房間/進入遊戲 未完成)

2.編寫開始介面功能(RoomListUI/CreateRoomUI：UI邏輯 未完成)

3.編寫組隊大廳介面功能(InRoomUI：UI邏輯 未完成)

4.編寫物件生成功能(SpawnManager：生成玩家/ObjectPoolManager：生成武器 未完成)

5.編寫資料結構(PlayerNetworkData：玩家資料/WeaponDataList_SO：武器資料)

6.編寫網路物件池功能(NetworkObjectPool/NetworkObjectPoolRoot)

7.編寫機率計算功能(AlgorithmManager：通用機率)
