# Cuberite-DB-Bridge
A tcp server to bridge cuberite plugins with mysql.
I will update this with a better Example once im further along.

Example usage:

```lua
function Initialize(Plugin)
	Plugin:SetName("cuberiteDBBridge")
	Plugin:SetVersion(1)

  cPluginManager:AddHook(cPluginManager.HOOK_PLAYER_SPAWNED, MyOnPlayerSpawned);

	LOG("Initialised " .. Plugin:GetName() .. " v." .. Plugin:GetVersion());
	return true
end

local LinkCallbacks =
{
	OnConnected = function (a_TCPLink)
    a_TCPLink:Send("CREATE TABLE Users (UserID INT PRIMARY KEY AUTO_INCREMENT, Username VARCHAR(255), Password VARCHAR(255)) ENGINE=INNODB;")
	end,

	OnError = function (a_TCPLink, a_ErrorCode, a_ErrorMsg)
		
	end,

	OnReceivedData = function (a_TCPLink, a_Data)
		--Must send this in order to make another connection.
		a_TCPLink:Send("Close Connection")
	end,

	OnRemoteClosed = function (a_TCPLink)
		
	end,
}

function MyOnPlayerSpawned(Player)
  
  local Server = cNetwork:Connect("127.0.0.1", 13000, LinkCallbacks);
  
end
```
