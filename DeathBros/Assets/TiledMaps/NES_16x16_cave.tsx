<?xml version="1.0" encoding="UTF-8"?>
<tileset version="1.2" tiledversion="1.2.0" name="NES_16x16_cave" tilewidth="16" tileheight="16" tilecount="256" columns="16">
 <image source="../Sprites/Tiles/NES_16x16_cave.png" width="256" height="256"/>
 <terraintypes>
  <terrain name="purple_ground" tile="17"/>
  <terrain name="dirt_ground" tile="81"/>
 </terraintypes>
 <tile id="0" terrain=",,,0"/>
 <tile id="1" terrain=",,0,0"/>
 <tile id="2" terrain=",,0,"/>
 <tile id="4" terrain="0,0,0,"/>
 <tile id="5" terrain="0,0,,0"/>
 <tile id="16" terrain=",0,,0"/>
 <tile id="17" terrain="0,0,0,0"/>
 <tile id="18" terrain="0,,0,"/>
 <tile id="20" terrain="0,,0,0"/>
 <tile id="21" terrain=",0,0,0"/>
 <tile id="32" terrain=",0,,"/>
 <tile id="33" terrain="0,0,,"/>
 <tile id="34" terrain="0,,,"/>
 <tile id="64" terrain=",,,1"/>
 <tile id="65" terrain=",,1,1"/>
 <tile id="66" terrain=",,1,"/>
 <tile id="68" terrain="1,1,1,"/>
 <tile id="69" terrain="1,1,,1"/>
 <tile id="80" terrain=",1,,1"/>
 <tile id="81" terrain="1,1,1,1"/>
 <tile id="82" terrain="1,,1,"/>
 <tile id="84" terrain="1,,1,1"/>
 <tile id="85" terrain=",1,1,1"/>
 <tile id="96" terrain=",1,,"/>
 <tile id="97" terrain="1,1,,"/>
 <tile id="98" terrain="1,,,"/>
 <tile id="204">
  <objectgroup draworder="index">
   <object id="1" x="0" y="16">
    <polygon points="0,0 16,-8 16,0"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="205">
  <objectgroup draworder="index">
   <object id="1" x="0" y="8">
    <polyline points="0,0 16,-8 16,8 0,8 0,0"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="206">
  <objectgroup draworder="index">
   <object id="1" x="0" y="0">
    <polygon points="0,0 16,8 16,16 0,16"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="207">
  <objectgroup draworder="index">
   <object id="1" x="0" y="8">
    <polyline points="0,0 16,8 0,8 0,0"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="220">
  <objectgroup draworder="index">
   <object id="1" x="0" y="0">
    <polygon points="0,0 16,8 16,0"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="221">
  <objectgroup draworder="index">
   <object id="1" x="0" y="8">
    <polygon points="0,0 16,8 16,-8 0,-8"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="222">
  <objectgroup draworder="index">
   <object id="1" x="0" y="16">
    <polygon points="0,0 16,-8 16,-16 0,-16"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="223">
  <objectgroup draworder="index">
   <object id="1" x="0" y="8">
    <polygon points="0,0 16,-8 0,-8"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="237">
  <objectgroup draworder="index">
   <object id="1" x="0" y="16">
    <polygon points="0,0 16,-16 16,0"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="238">
  <objectgroup draworder="index">
   <object id="1" x="0" y="16">
    <polygon points="0,0 0,-16 16,0"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="251">
  <objectgroup draworder="index">
   <object id="1" x="0" y="0" width="16" height="2"/>
  </objectgroup>
 </tile>
 <tile id="253">
  <objectgroup draworder="index">
   <object id="1" x="0" y="0">
    <polygon points="0,0 16,16 16,0"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="254">
  <objectgroup draworder="index">
   <object id="1" x="0" y="16">
    <polygon points="0,0 16,-16 0,-16"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="255">
  <objectgroup draworder="index">
   <object id="1" x="0" y="0" width="16" height="16"/>
  </objectgroup>
 </tile>
</tileset>
