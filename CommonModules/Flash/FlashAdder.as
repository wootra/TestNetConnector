package{
	import flash.display.MovieClip;
	import flash.display.Sprite;
	import flash.events.ContextMenuEvent;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.external.ExternalInterface;
	import flash.ui.ContextMenu;
	import flash.ui.ContextMenuItem;

	public dynamic class FlashAdder extends Sprite
	{
		public function FlashAdder(){
			
			this.addEventListener(MouseEvent.CLICK,FA_onClick);
			this.addEventListener(MouseEvent.MOUSE_WHEEL,FA_onWheel);
			
			ExternalInterface.addCallback("setValue",setValue);
			ExternalInterface.addCallback("getValue",getValue);
			ExternalInterface.addCallback("addRightMenu",addRightMenu);
					
		}
	
		//private dynamic function dyFunc(evt:Event){}
		public function addRightMenu(menuText:String, funcName:String, sepBefore:Boolean=false, mekeNew:Boolean= false){
			
			this[funcName] = function(evt:Event){
				externalFunc(funcName);
			}
			
			//dyFunc[funcName] = function(evt:Event);
			FA_addRightClickItem(menuText, this[funcName], sepBefore, mekeNew);
		}
		
		function externalFunc(funcName:String){
			ExternalInterface.call(funcName);
		}
		public function FA_addRightClickItem(text:String, func:Function , seperateFromBefore:Boolean, makeNewMenu:Boolean){
			var myMenu:ContextMenu;
			if(makeNewMenu==true) myMenu = new ContextMenu();
			else myMenu = contextMenu;
			
			var item1:ContextMenuItem = new ContextMenuItem(text);
			item1.separatorBefore = seperateFromBefore;
			myMenu.customItems.push(item1);
			item1.addEventListener(ContextMenuEvent.MENU_ITEM_SELECT, func);
			
			myMenu.hideBuiltInItems();
			contextMenu = myMenu;
		}
											 
		function FA_onClick(sender:Object){
			trace("click!");
			var movie:MouseEvent = sender as MouseEvent;
			
			var btn:String = "left";
			var x:Number = movie.localX;
			var y:Number = movie.localY;
			var delta:int = movie.delta;
			var numOfClicked:int = 1;

			ExternalInterface.call("click",btn, numOfClicked, x, y, delta);
		}
		function FA_onWheel(sender:Object){
			var movie:MouseEvent = sender as MouseEvent;
			
			var btn:String = "delta";
			var x:int = movie.localX;
			var y:int = movie.localY;
			var delta:int = movie.delta;
			var numOfClicked:int = 1;

			ExternalInterface.call("click",btn, numOfClicked, x, y, delta);
		}
		public function getValue():Number{return 0;}
		public function setValue(arg:Number,...rest):void{}
	}
}