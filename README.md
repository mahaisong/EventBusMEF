 
# EventBusMEF
一个MEF扫描，内部EventBus注入、内置发布订阅invoke执行、动态参数的功能--------------大量的反射  

4个主要的类：  
RegistryEventTypes 字典类：1.主要将event和对应的eventhandler 自动扫描并放入字典中。2.同时开发了注入能力--跨程序集被MEF引入的注入。  
BioEventBus:核心类，有1个queue队列，1.有1个线程自动执行Dequeue出队列并消费func。   2.对外开放了enqueue入队列能力。  
BioPublisher：发布类： 1.通过将event作为1个属性，放入1个新的message封装类中。 2.调用BioEventBus 的发布消息的方法。  
BioPConsumer：消费类： 1.单例，自动调用BioEventBus的 消费func 能力【具体的func】。 2.实现【具体的func】----找到RegistryEventTypes 对应的eventhandler--创建实例 并invoke。  

注意MEF：    
通过MEF，自动将其他dll文件中的init方法暴露。 将自己类库中的event和eventhandler 关系挂载到统一的 RegistryEventTypes 字典类中。    
既：N个dll文件，都是在同一个字典中存在。     

应用：  
1.注入：MEF加载其他类库、指定每个类库的init----------------既将 此dll下的event和eventhandler 注入了统一的字典中。  
2.使用：通过字符串、event 都可以发布 event实例obj对象。   
甚至还可以扩展对应的参数并赋值。  

传输过程：是-- event实例obj对----》封装在message里面---->publish---------------------------while消费Consumer---》message反序列化出event实例对象obj----》去字典根据event找到的handler----》invoke执行handler【含参数】  
特点：不考虑主程序：A设备可以根据event的定义，通用的方式调用B设备eventhandler。设备调用设备。非常灵活。  

改进点1： 怎么同步  
因为投递和执行完成是分离的，所以是完全异步。但是这里如果想要同步卡住怎么弄？ 就可以执行不投递--直接找到并invoke--等待完成。  
改进点2：  
异步怎么返回值---》统一的新的参数字典。  


升级：  
发布的消息，当前是内存的queue，后面可以升级MQ组件--比如rabbitmq等。  
