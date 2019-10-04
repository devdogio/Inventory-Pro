<p align="center">
    <img src="https://i.imgur.com/39SLog4.jpg" alt="Inventory Pro">
</p>
<h3 align="center" style="text-align:center;">
	The best-selling Inventory System for Unity - now free and open-source!
</h3>
<p align="center">
	Inventory Pro was originally developed by Devdog and has sold 100,000+ licenses on the <a href="https://assetstore.unity.com/publishers/3727">Unity Asset Store</a>.
</p>

<hr>

<h3 align="center" style="text-align:center;">
	Other Devdog Publishing Tools:
</h3>
<p align="center">	
	<a href="https://odininspector.com" target="_blank">
		<img src="https://i.imgur.com/mIPtgxG.png" alt="Odin Inspector">
	</a>
	<a href="https://assetstore.unity.com/packages/templates/systems/rucksack-ultimate-inventory-system-114921" target="_blank">
		<img src="https://i.imgur.com/IxKDtuv.png" alt="Rucksack">
</p>
<hr>

## Important Links
<p align="center">
	<b>Discuss Inventory Pro with the rest of the community on Discord</b></p>
<p align="center">
	<a href="https://discord.gg/AgDmStu">
		<img src="https://discordapp.com/api/guilds/355444042009673728/embed.png" alt="Discord server"></a></p>

<p align="center">
	<b>Read the Inventory Pro Documentation</b></p>
<p align="center">
	<a href="https://inventory-pro-docs.readthedocs.io/en/latest/">
		<img src="https://i.imgur.com/0uTxaXy.png" alt="Documentation"></a></p>

## Why is Inventory Pro being open-sourced?

After years on the Asset Store, we're open-sourcing Inventory Pro because we're moving on to new projects and tools. You can read more in our blog post.

100% in the hands of the community, we hope to see the tool flourish, development continue, and any upcoming bugs fixed (the tool should be bug-free as-is).

## Getting started

Clone the repository + submodules, or download the zip file + [the general library](https://github.com/devdogio/general) and place it in Assets/Devdog.

Integrations stored in the Integrations folder may have to be removed, if you do not have these plugins in your project.

## What's included with Inventory Pro?

#### Multiple Inventories
The extensive multiple inventories system allows you to .e.g setup 2 player-inventories; one for consumable loot drops (e.g. an apple or a potion), and one for other types of loot. Once setup, the system will automatically move looted items to the appropriate inventory.

The system doesn't restrict the amount of inventories you can create, so go nuts with your creativity!

#### Advanced currency system
The powerful currency editor allows you to define all your game's currencies; for use at vendors, in crafting blueprints etc. Each currency can even contain a set of conversions. For example, imagine a game with 3 currencies; Gold, Silver, and copper. When running out of copper, the system can convert the silver currency down to copper, essentially re-filling it from a more valuable currency that can be converted down.

#### Full mobile support
Inventory Pro works across PC, mobile, and consoles, allowing you full freedom to port games using Inventory Pro to most platforms.

#### Crafting
Inventory Pro makes it easy to create crafting systems. The standard crafting system can of course be inherited from and overridden to modify certain aspects of the crafting process. For example, if you want to add another collection to scan for potential items the user would require.

#### Restrictions
Restrict your collections to types, weight, dragging, (un)stacking and more. This allows you to create a collection that can only hold consumable items, only quest items or both.

#### Item Layouts
Allow each item to take up a specified amount of space in your collections. For example, allow a shield to take up a 2x2 slot while an apple only takes a 1x1 slot.

#### Cooldowns
Categories can be given a cooldown, all items sharing this category will all go into cooldown when an item from this category is used. Of course you can override the cooldown per item.

#### Collections
Easily create your own collections, simply drag on a component, play with the settings in the inspector and voila. The skillbar uses references, and sums up the total amount of items in the inventories.

#### Filters
Filter collections (any window with items in it). For example when you create multiple inventories (which is very possible) you can filter one to only allow quest items. 

#### Vendors
Buying and selling from vendors, such as an in-game NPC, is made easy - and with Inventory Pro, you can tweak each vendor right inside the Unity inspector.

It doesn't get easier than that.

#### Cleanup
Sort, re-stack and tidy up your collections with a simlpe click of a button. The sorting algorithm can be tweaked to your liking to sort based on any paramter or type.

#### Use items to restore stats
Easily setup a system to e.g. recover health when your player eats an apple. The system is very extensible and can be used to recover stats in many different game-genres.

#### Controller support
Inventory pro's controller support means the UI is controller friendly. There's even a demo scene for this.
