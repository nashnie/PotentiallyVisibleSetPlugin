# PotentiallyVisibleSetPlugin
https://en.wikipedia.org/wiki/Potentially_visible_set

# 步骤如下
step1 地图切割成Tile，Tile切割成Portal（视口）和Cell（大物件、中物件、小物件）；<br>
step2 检测场景物件所属Cell，一个物件可以属于多个Cell；<br>
step3 利用蒙特卡洛方法Portal随机一些起点，Cell随机一些终点（根据大、中、小随机不同数量的点）；<br>
step4 双层遍历每个Portal和Cell，射线检测是否阻挡，如果阻挡，判断阻挡物是否被在当前检测Cell内，如果是，Cell可见；（注，当前设置两个高度检测）<br>
step5 保存检测数据，运行时加载解析，根据玩家位置检测当前Portal，然后遍历场景所有Cell内Item，判断显示和隐藏；<br>

# 综述
此pvs方案相比于Unity OcclusionCulling <br>
优点：更适合大地图比如5000米x5000米，pvs生成data基于Tile，动态加载，占用内存忽略不计，同时CPU消耗也极少；<br>
缺点：剔除没有Unity精细同时烘焙时间很长...；<br>

# Feature
bytes存储数据替代xml
