using GameData.MapElement;

public interface IGameObjectControl <T>
    where T: MapElementBase
{
    void SyncMapElementStatus(T element);
}