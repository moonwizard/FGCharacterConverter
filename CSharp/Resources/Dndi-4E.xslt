<?xml version="1.0" encoding="utf-8"?>
<!-- Copyright (c) 2012, SmiteWorks USA LLC -->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:output method="xml" indent="yes"/>
  <xsl:variable name="upperCase" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'"/>
  <xsl:variable name="lowerCase" select="'abcdefghijklmnopqrstuvwxyz'"/>
  <xsl:variable name="characterlevel"   select="number(normalize-space(/D20Character/CharacterSheet/Details/Level))"/>
  <xsl:variable name="halflevel"   select="floor($characterlevel div 2)"/>
  <xsl:variable name="armor"     select="/D20Character/CharacterSheet/LootTally/loot[@equip-count='1']/RulesElement[@type='Armor']/@name"/>
  <xsl:template match="/D20Character">
    <xsl:apply-templates select="CharacterSheet"/>
  </xsl:template>
  <xsl:template match="/D20Character/CharacterSheet">
    <root version="2.8">
      <character>
        <actionpoints type="number">1</actionpoints>
        <appearance type="string">
          <xsl:value-of select="normalize-space(Details/Appearance)"/>
        </appearance>
        <abilities>
          <xsl:apply-templates select="
        StatBlock/Stat[alias/@name='Strength']     |
        StatBlock/Stat[alias/@name='Constitution'] |
        StatBlock/Stat[alias/@name='Dexterity']    |
        StatBlock/Stat[alias/@name='Intelligence'] |
        StatBlock/Stat[alias/@name='Wisdom']       |
        StatBlock/Stat[alias/@name='Charisma'] "/>
        </abilities>
        <age type="string">
          <xsl:value-of select="normalize-space(Details/Age)"/>
        </age>
        <alignment type="string">
          <xsl:value-of select="normalize-space(Details/Alignment)"/>
        </alignment>
        <attacks>
          <base>
            <total type="number">0</total>
          </base>
          <melee>
            <misc type="number">0</misc>
            <temporary type="number">0</temporary>
            <total type="number">0</total>
          </melee>
          <ranged>
            <misc type="number">0</misc>
            <temporary type="number">0</temporary>
            <total type="number">0</total>
          </ranged>
        </attacks>
        <class>
          <base type="string">|CC.DNDI.CLASS.BASE|</base>
          <paragon type="string">|CC.DNDI.CLASS.PARAGON|</paragon>
          <epic type="string">|CC.DNDI.CLASS.EPIC|</epic>
        </class>
        <coins>|CC.DNDI.COINS|</coins>
        <defenses>
          <ac>|CC.DNDI.DEFENSES.AC|</ac>
          <reflex>|CC.DNDI.DEFENSES.REFLEX|</reflex>
          <will>|CC.DNDI.DEFENSES.WILL|</will>
          <fortitude>|CC.DNDI.DEFENSES.FORTITUDE|</fortitude>
          <save>
            <base type="number">
              <xsl:value-of select="StatBlock/Stat[@name='Saving Throws']/@value"/>
            </base>
            <total type="number">
              <xsl:value-of select="StatBlock/Stat[@name='Saving Throws']/@value"/>
            </total>
          </save>
        </defenses>
        <deity type="string">
          <xsl:value-of select="RulesElementTally/RulesElement[@type='Deity']/@name"/>
        </deity>
        <encumbrance>
          <armorcheckpenalty type="number">
            <xsl:value-of select="StatBlock/Stat[alias/@name='Armor Penalty']/@value"/>
          </armorcheckpenalty>
          <heavyarmor type="number">|CC.DNDI.ENCUMBRANCE.HEAVYARMOR|</heavyarmor>
          <load       type="number"/>
          <normalload type="number"/>
          <heavyload  type="number"/>
          <dragload   type="number"/>
        </encumbrance>
        <exp type="number">
          <xsl:value-of select="normalize-space(Details/Experience)"/>
        </exp>
        <expneeded type="number">
          <!--
          <xsl:call-template name="expneeded">
            <xsl:with-param name="xp" select="normalize-space(Details/Experience)"/>
          </xsl:call-template>
          -->
          <xsl:value-of select="StatBlock/Stat[alias/@name='XP Needed']/@value"/>
        </expneeded>
        <featlist>|CC.DNDI.FEATLIST|</featlist>
        <gender type="string">
          <xsl:value-of select="RulesElementTally/RulesElement[@type='Gender']/@name"/>
        </gender>
        <height type="string">
          <xsl:value-of select="normalize-space(Details/Height)"/>
        </height>
        <hp>
          <xsl:variable name="hp" select="number(normalize-space(StatBlock/Stat[alias/@name='Hit Points']/@value))"/>
          <total type="number">
            <xsl:value-of select="$hp"/>
          </total>
          <surgemodifier type="number">
            <xsl:value-of select="number(translate(string(number(normalize-space(StatBlock/Stat[alias/@name='Healing Surge value']/@value))), 'Na', '0'))"/>
          </surgemodifier>
          <bloodied type="number">
            <xsl:value-of select="floor($hp div 2)"/>
          </bloodied>
          <surgesmax type="number">
            <xsl:value-of select="StatBlock/Stat[alias/@name='Healing Surges']/@value"/>
          </surgesmax>
          <surge type="number"/>
          <surgesused type="number">0</surgesused>
          <temporary type="number">0</temporary>
          <wounds type="number">0</wounds>
          <faileddeathsaves type="number">0</faileddeathsaves>
          <maxdeathsaves type="number">
            <xsl:value-of select="StatBlock/Stat[alias/@name='Death Saves Count']/@value"/>
          </maxdeathsaves>
          <secondwind type="number"/>
        </hp>
        <initiative>
          <misc type="number">
            <xsl:value-of select="StatBlock/Stat[alias/@name='Initiative Misc']/@value"/>
          </misc>
          <total type="number">
            <xsl:value-of select="StatBlock/Stat[alias/@name='Initiative']/@value"/>
          </total>
          <temporary type="number"/>
        </initiative>
        <inventorylist>|CC.DNDI.INVENTORYLIST|</inventorylist>
        <languagelist>
          <xsl:apply-templates select="RulesElementTally/RulesElement[@type='Language']">
            <xsl:with-param name="inner-element-name" select="'name'"/>
          </xsl:apply-templates>
        </languagelist>
        <level type="number">
          <xsl:value-of select="$characterlevel"/>
        </level>
        <levelbonus type="number">
          <xsl:value-of select="$halflevel"/>
        </levelbonus>
        <meleemodifier type="number"/>
        <name type="string">
          <xsl:value-of select="normalize-space(Details/name)"/>
        </name>
        <notes type="string">
          <xsl:value-of select="normalize-space(Details/Notes)"/>
        </notes>
        <powerfocus>
          <implement>
            <order type="number">0</order>
          </implement>
          <weapon>
            <order type="number">0</order>
          </weapon>
        </powerfocus>
        <powermode type="string">standard</powermode>
        <preparationmode type="number">0</preparationmode>
        <passiveinsight type="number">
          <xsl:value-of select="StatBlock/Stat[alias/@name='Passive Insight']/@value"/>
        </passiveinsight>
        <passiveperception type="number">
          <xsl:value-of select="StatBlock/Stat[alias/@name='Passive Perception']/@value"/>
        </passiveperception>
        <proficiencyarmor>|CC.DNDI.PROFICIENCYARMOR|</proficiencyarmor>
        <proficiencyweapon>|CC.DNDI.PROFICIENCYWEAPON|</proficiencyweapon>
        <race type="string">
          <xsl:value-of select="normalize-space(/D20Character/CharacterSheet/RulesElementTally/RulesElement[@type='Race']/@name)"/>
        </race>
        <senses type="string">|CC.DNDI.SENSES|</senses>
        <size type="string">
          <xsl:value-of select="RulesElementTally/RulesElement[@type='Size']/@name"/>
        </size>
        <weight type="string">
          <xsl:value-of select="normalize-space(Details/Weight)"/>
        </weight>
        <powers>|CC.DNDI.POWERS|</powers>
        <skilllist>
          <xsl:apply-templates select="
        StatBlock/Stat[alias/@name='Arcana'] |
        StatBlock/Stat[alias/@name='Diplomacy'] |
        StatBlock/Stat[alias/@name='Nature'] |
        StatBlock/Stat[alias/@name='Endurance'] |
        StatBlock/Stat[alias/@name='Thievery'] |
        StatBlock/Stat[alias/@name='Acrobatics'] |
        StatBlock/Stat[alias/@name='Religion'] |
        StatBlock/Stat[alias/@name='Athletics'] |
        StatBlock/Stat[alias/@name='Insight'] |
        StatBlock/Stat[alias/@name='Bluff'] |
        StatBlock/Stat[alias/@name='Heal'] |
        StatBlock/Stat[alias/@name='Streetwise'] |
        StatBlock/Stat[alias/@name='Dungeoneering'] |
        StatBlock/Stat[alias/@name='Perception'] |
        StatBlock/Stat[alias/@name='Stealth'] |
        StatBlock/Stat[alias/@name='Intimidate'] |
        StatBlock/Stat[alias/@name='History']
      "/>
        </skilllist>
        <speed>
          <base type="number">
            <xsl:value-of select="number(translate(number(StatBlock/Stat[alias/@name='Speed']/statadd[@Level &gt; 0]/@value), 'Na', '0'))"/>
          </base>
          <armor type="number">|CC.DNDI.SPEED.ARMOR|</armor>
          <misc type="number">|CC.DNDI.SPEED.MISC|</misc>
          <temporary type="number">0</temporary>
          <final type="number">
            <xsl:value-of select="StatBlock/Stat[alias/@name='Speed']/@value"/>
          </final>
        </speed>
        <special type="string">|CC.DNDI.SPECIAL|</special>
        <specialabilitylist>|CC.DNDI.SPECIALABILITYLIST|</specialabilitylist>
        <weaponlist>|CC.DNDI.WEAPONLIST|</weaponlist>
      </character>
    </root>
  </xsl:template>
  <xsl:template match="Stat">
    <xsl:choose>
      <xsl:when test="alias/@name='Strength' or alias/@name='Constitution' or alias/@name='Dexterity' or alias/@name='Intelligence' or alias/@name='Wisdom' or alias/@name='Charisma'">
        <xsl:variable name="attr"  select="translate(alias[1]/@name, $upperCase, $lowerCase)"/>
        <xsl:variable name="score" select="floor( ( number(@value)-10 ) div 2)"/>
        <xsl:element name="{$attr}">
          <score type="number">
            <xsl:value-of select="@value"/>
          </score>
          <bonus type="number">
            <xsl:value-of select="$score"/>
          </bonus>
          <check type="number">
            <xsl:value-of select="$score + $halflevel"/>
          </check>
          <bonusmodifier type="number">0</bonusmodifier>
        </xsl:element>
      </xsl:when>
      <xsl:when test="alias/@name='Arcana' or
        alias/@name='Diplomacy' or
        alias/@name='Nature' or
        alias/@name='Endurance' or
        alias/@name='Thievery' or
        alias/@name='Acrobatics' or
        alias/@name='Religion' or
        alias/@name='Athletics' or
        alias/@name='Insight' or
        alias/@name='Bluff' or
        alias/@name='Heal' or
        alias/@name='Streetwise' or
        alias/@name='Dungeoneering' or
        alias/@name='Perception' or
        alias/@name='Stealth' or
        alias/@name='Intimidate' or
        alias/@name='History'">
        <xsl:variable name="num">id-<xsl:number value="position()" format="00001"/></xsl:variable>
        <xsl:element name="{$num}">
          <armorcheckmultiplier type="number">0</armorcheckmultiplier>
          <classskill type="number">0</classskill>
          <label type="string">
            <xsl:value-of select="alias/@name"/>
          </label>
          <misc type="number">
            <xsl:value-of select="D20Character/CharacterSheet/Stat[alias/@name=concat(current()/@name, ' Misc')]/@value"/> 
          </misc>
          <name type="string">
            <xsl:value-of select="alias/@name"/>
          </name>
          <showonminisheet type="number">1</showonminisheet>
          <trained type="number">
            <xsl:choose>
              <xsl:when test="../Stat[@name=concat(current()/alias/@name,' Trained')]/@value &gt; 0">1</xsl:when>
              <xsl:otherwise>0</xsl:otherwise>
            </xsl:choose>
          </trained>
        </xsl:element>
      </xsl:when>
      <xsl:when test="contains(alias/@name, 'Resist:') or contains(alias/@name, 'Vulnerable:') or contains(alias/@name, 'Immune:')">
        <xsl:value-of select="substring-before(alias/@name, ':')"/>
        <xsl:text> </xsl:text>
        <xsl:value-of select="@value"/>
        <xsl:text> </xsl:text>
        <xsl:value-of select="substring-after(alias/@name, ':')"/>
        <xsl:text> </xsl:text>
      </xsl:when>
    </xsl:choose>
  </xsl:template>
  <xsl:template match="RulesElement">
    <xsl:param name="reference-class"  select="false()"/>
    <xsl:param name="reference-record" select="false()"/>
    <xsl:param name="inner-element-name" select="'value'"/>
    <xsl:variable name="num">id-<xsl:number value="position()" format="00001"/></xsl:variable>
    <xsl:element name="{$num}">
      <xsl:if test="$reference-class and $reference-record">
        <shortcut type="windowreference">
          <class>
            <xsl:value-of select="$reference-class"/>
          </class>
          <recordname>
            <xsl:value-of select="$reference-record"/>
          </recordname>
        </shortcut>
      </xsl:if>
        <xsl:element name="{$inner-element-name}">
          <xsl:attribute name="type">string</xsl:attribute>
          <xsl:value-of select="@name"/>
        </xsl:element>
    </xsl:element>
  </xsl:template>
</xsl:stylesheet>