<?xml version="1.0" encoding="utf-8"?>
<!-- Copyright (c) 2012, SmiteWorks USA LLC -->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:output method="xml" indent="yes"/>
  <xsl:template match="/child::*">
    <xsl:variable name="sizemod" select="armorclass/@fromsize"/>
    <xsl:variable name="meleeablmod" select="attributes/attribute[@name='Strength']/attrbonus/@modified"/>
    <xsl:variable name="rangedablmod" select="attributes/attribute[@name='Dexterity']/attrbonus/@modified"/>
    <xsl:variable name="grappleablmod" select="attributes/attribute[@name='Strength']/attrbonus/@modified"/>
    <xsl:variable name="initiativeablmod" select="attributes/attribute[@name='Dexterity']/attrbonus/@modified"/>
    <root version="2.8" ccversion="2.8.1.C">
      <character>
        <abilities>
          <charisma>
            <bonus type="number">
              <xsl:value-of select="attributes/attribute[@name='Charisma']/attrbonus/@modified"/>
            </bonus>
            <bonusmodifier type="number"/>
            <damage type="number"/>
            <score type="number">
              <xsl:value-of select="attributes/attribute[@name='Charisma']/attrvalue/@modified"/>
            </score>
          </charisma>
          <constitution>
            <bonus type="number">
              <xsl:value-of select="attributes/attribute[@name='Constitution']/attrbonus/@modified"/>
            </bonus>
            <bonusmodifier type="number"/>
            <damage type="number"/>
            <score type="number">
              <xsl:value-of select="attributes/attribute[@name='Constitution']/attrvalue/@modified"/>
            </score>
          </constitution>
          <dexterity>
            <bonus type="number">
              <xsl:value-of select="attributes/attribute[@name='Dexterity']/attrbonus/@modified"/>
            </bonus>
            <bonusmodifier type="number"/>
            <damage type="number"/>
            <score type="number">
              <xsl:value-of select="attributes/attribute[@name='Dexterity']/attrvalue/@modified"/>
            </score>
          </dexterity>
          <intelligence>
            <bonus type="number">
              <xsl:value-of select="attributes/attribute[@name='Intelligence']/attrbonus/@modified"/>
            </bonus>
            <bonusmodifier type="number"/>
            <damage type="number"/>
            <score type="number">
              <xsl:value-of select="attributes/attribute[@name='Intelligence']/attrvalue/@modified"/>
            </score>
          </intelligence>
          <strength>
            <bonus type="number">
              <xsl:value-of select="attributes/attribute[@name='Strength']/attrbonus/@modified"/>
            </bonus>
            <bonusmodifier type="number"/>
            <damage type="number"/>
            <score type="number">
              <xsl:value-of select="attributes/attribute[@name='Strength']/attrvalue/@modified"/>
            </score>
          </strength>
          <wisdom>
            <bonus type="number">
              <xsl:value-of select="attributes/attribute[@name='Wisdom']/attrbonus/@modified"/>
            </bonus>
            <bonusmodifier type="number"/>
            <damage type="number"/>
            <score type="number">
              <xsl:value-of select="attributes/attribute[@name='Wisdom']/attrvalue/@modified"/>
            </score>
          </wisdom>
        </abilities>
        <ac>
          <sources>
            <xsl:variable name="dodge" select="format-number(armorclass/@fromdodge, '0')"/>
            <xsl:variable name="miscval" select="format-number(armorclass/@frommisc, '0')"/>
            <abilitymod type="number">
              <xsl:value-of select="armorclass/@fromdexterity"/>
            </abilitymod>
            <armor type="number">
              <xsl:value-of select="armorclass/@fromarmor"/>
            </armor>
            <deflection type="number">
              <xsl:value-of select="armorclass/@fromdeflect"/>
            </deflection>
            <dodge type="number">
              <xsl:value-of select="armorclass/@fromdodge"/>
            </dodge>
            <misc type="number">
              <xsl:value-of select="armorclass/@frommisc"/>
            </misc>
            <naturalarmor type="number">
              <xsl:value-of select="armorclass/@fromnatural"/>
            </naturalarmor>
            <shield type="number">
              <xsl:value-of select="armorclass/@fromshield"/>
            </shield>
            <size type="number">
              <xsl:value-of select="$sizemod"/>
            </size>
            <temporary type="number"/>
          </sources>
          <totals>
            <cmd type="number"/>
            <flatfooted type="number">
              <xsl:value-of select="armorclass/@flatfooted"/>
            </flatfooted>
            <general type="number">
              <xsl:value-of select="armorclass/@ac"/>
            </general>
            <touch type="number">
              <xsl:value-of select="armorclass/@touch"/>
            </touch>
          </totals>
        </ac>
        <appearance type="string"/>
        <age type="string">
          <xsl:value-of select="personal/@age"/>
        </age>
        <alignment type="string">
          <xsl:value-of select="alignment/@name"/>
        </alignment>
        <attackbonus>
          <xsl:variable name="base" select="attack/@baseattack"/>
          <base type="number">
            <xsl:value-of select="$base"/>
          </base>
          <grapple>
            <abilitymod type="number">
              <xsl:value-of select="$grappleablmod"/>
            </abilitymod>
            <misc type="number">|CC.HL.ATTACKBONUS.GRAPPLE.MISC|</misc>
            <size type="number">|CC.HL.ATTACKBONUS.GRAPPLE.SIZE|</size>
            <temporary type="number"/>
            <total type="number">|CC.HL.ATTACKBONUS.GRAPPLE.TOTAL|</total>
          </grapple>
          <melee>
            <abilitymod type="number">
              <xsl:value-of select="$meleeablmod"/>
            </abilitymod>
            <misc type="number">|CC.HL.ATTACKBONUS.MELEE.MISC|</misc>
            <size type="number">
              <xsl:value-of select="$sizemod"/>
            </size>
            <temporary type="number"/>
            <total type="number">|CC.HL.ATTACKBONUS.MELEE.TOTAL|</total>
          </melee>
          <ranged>
            <abilitymod type="number">
              <xsl:value-of select="$rangedablmod"/>
            </abilitymod>
            <misc type="number">|CC.HL.ATTACKBONUS.RANGED.MISC|</misc>
            <size type="number">
              <xsl:value-of select="$sizemod"/>
            </size>
            <temporary type="number"/>
            <total type="number">|CC.HL.ATTACKBONUS.RANGED.TOTAL|</total>
          </ranged>
        </attackbonus>
        <classes>|CC.HL.CLASSES|</classes>
        <coins>
          <slot1>
            <amount type="number">
              <xsl:value-of select="money/@pp"/>
            </amount>
            <name type="string">pp</name>
          </slot1>
          <slot2>
            <amount type="number">
              <xsl:value-of select="money/@gp"/>
            </amount>
            <name type="string">gp</name>
          </slot2>
          <slot3>
            <amount type="number">
              <xsl:value-of select="money/@sp"/>
            </amount>
            <name type="string">sp</name>
          </slot3>
          <slot4>
            <amount type="number">
              <xsl:value-of select="money/@cp"/>
            </amount>
            <name type="string">cp</name>
          </slot4>
          <slot5>
            <amount type="number"/>
            <name type="string"/>
          </slot5>
          <slot6>
            <amount type="number"/>
            <name type="string"/>
          </slot6>
        </coins>
        <combattoken type="token"/>
        <defenses>
          <damagereduction type="string"/>
          <sr>
            <base type="number"/>
            <misc type="number"/>
            <temporary type="number"/>
            <total type="number"/>
          </sr>
        </defenses>
        <deity type="string">
          <xsl:value-of select="deity/@name"/>
        </deity>
        <encumbrance>
          <armorcheckpenalty type="number">
            <xsl:value-of select="penalties/penalty[@name='Armor Check Penalty']/@value"/>
          </armorcheckpenalty>
          <armormaxstatbonus type="number">
            <xsl:value-of select="penalties/penalty[@name='Max Dex Bonus']/@value"/>
          </armormaxstatbonus>
          <armormaxstatbonusactive type="number"/>
          <heavyload type="number">
            <xsl:value-of select="encumbrance/@heavy"/>
          </heavyload>
          <liftoffground type="number"/>
          <liftoverhead type="number"/>
          <lightload type="number">
            <xsl:value-of select="encumbrance/@light"/>
          </lightload>
          <load type="number">
            <xsl:value-of select="encumbrance/@carried"/>
          </load>
          <mediumload type="number">
            <xsl:value-of select="encumbrance/@medium"/>
          </mediumload>
          <pushordrag type="number"/>
          <spellfailure type="number"/>
        </encumbrance>
        <exp type="number">
          <xsl:value-of select="xp/@total"/>
        </exp>
        <expneeded type="number"/>
        <featlist>|CC.HL.FEATLIST|</featlist>
        <gender type="string">
          <xsl:value-of select="personal/@gender"/>
        </gender>
        <height type="string">
          <xsl:value-of select="personal/charheight/@text"/>
        </height>
        <hp>
          <nonlethal type="number">
            <xsl:value-of select="health/@nonlethal"/>
          </nonlethal>
          <surgesused type="number"/>
          <temporary type="number"/>
          <total type="number">
            <xsl:value-of select="health/@hitpoints"/>
          </total>
          <wounds type="number">
            <xsl:value-of select="health/@damage"/>
          </wounds>
        </hp>
        <initiative>
          <abilitymod type="number">
            <xsl:value-of select="initiative/@attrtext"/>
          </abilitymod>
          <misc type="number">
            <xsl:value-of select="initiative/@misctext"/>
          </misc>
          <temporary type="number"/>
          <total type="number">
            <xsl:value-of select="initiative/@total"/>
          </total>
        </initiative>
        <inventorylist>|CC.HL.INVENTORYLIST|</inventorylist>
        <languagelist>|CC.HL.LANGUAGELIST|</languagelist>
        <level type="number">
          <xsl:value-of select="classes/@level"/>
        </level>
        <name type="string">
          <xsl:value-of select="@name"/>
        </name>
        <notes type="string"/>
        <proficiencyarmor>|CC.HL.PROFICIENCYARMOR|</proficiencyarmor>
        <proficiencyweapon>|CC.HL.PROFICIENCYWEAPON|</proficiencyweapon>
        <race type="string">
          <xsl:value-of select="race/@name"/>
        </race>
        <saves>
          <fortitude>
            <abilitymod type="number">
              <xsl:value-of select="saves/save[@name='Fortitude Save']/@fromattr"/>
            </abilitymod>
            <base type="number">
              <xsl:value-of select="saves/save[@name='Fortitude Save']/@base"/>
            </base>
            <misc type="number">|CC.HL.SAVES.FORTITUDE.MISC|</misc>
            <temporary type="number"/>
            <total type="number">
              <xsl:value-of select="saves/save[@name='Fortitude Save']/@fromattr"/>
            </total>
          </fortitude>
          <reflex>
            <abilitymod type="number">
              <xsl:value-of select="saves/save[@name='Reflex Save']/@fromattr"/>
            </abilitymod>
            <base type="number">
              <xsl:value-of select="saves/save[@name='Reflex Save']/@base"/>
            </base>
            <misc type="number">|CC.HL.SAVES.REFLEX.MISC|</misc>
            <temporary type="number"/>
            <total type="number">
              <xsl:value-of select="saves/save[@name='Reflex Save']/@fromattr"/>
            </total>
          </reflex>
          <will>
            <abilitymod type="number">
              <xsl:value-of select="saves/save[@name='Will Save']/@fromattr"/>
            </abilitymod>
            <base type="number">
              <xsl:value-of select="saves/save[@name='Will Save']/@base"/>
            </base>
            <misc type="number">|CC.HL.SAVES.WILL.MISC|</misc>
            <temporary type="number"/>
            <total type="number">
              <xsl:value-of select="saves/save[@name='Will Save']/@fromattr"/>
            </total>
          </will>
        </saves>
        <senses type="string">|CC.HL.SENSES|</senses>
        <size type="string">
          <xsl:value-of select="size/@name"/>
        </size>
        <skilllist>|CC.HL.SKILLLIST|</skilllist>
        <skillpoints>
          <unspent type="number"/>
        </skillpoints>
        <specialabilitylist>|CC.HL.SPECIALABILITYLIST|</specialabilitylist>
        <speed>
          <xsl:variable name="total" select="movement/speed/@value"/>
          <xsl:variable name="base" select="movement/basespeed/@value"/>
          <special type="string"/>
          <armor type="number"/>
          <base type="number">
            <xsl:value-of select="$base"/>
          </base>
          <final type="number">
            <xsl:value-of select="$total"/>
          </final>
          <misc type="number">
            <xsl:value-of select="$total - $base"/>
          </misc>
          <temporary type="number"/>
          <total type="number">
            <xsl:value-of select="$total"/>
          </total>
        </speed>
        <spellset>|CC.HL.SPELLSET|</spellset>
        <weaponlist>|CC.HL.WEAPONLIST|</weaponlist>
        <weight type="string">
          <xsl:value-of select="personal/charweight/@text"/>
        </weight>
      </character>
    </root>
  </xsl:template>
</xsl:stylesheet>
