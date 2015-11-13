namespace Contract
{
    public interface ICommandHandler<TCommand>
    {
        void Handle(TCommand command);
    }
}