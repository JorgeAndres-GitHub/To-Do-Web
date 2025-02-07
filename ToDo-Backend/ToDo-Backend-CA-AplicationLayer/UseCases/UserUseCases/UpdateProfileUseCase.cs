public class UpdateProfileUseCase<TDTO, TAuthResult>
{
    private readonly IAccountRepository<UserEntity, TAuthResult> _repository;

    public UpdateProfileUseCase(IAccountRepository<UserEntity, TAuthResult> repository)
    {
        _repository = repository
    }

    public async Task ExecuteAsync(int id)
    {
        await _repository.UpdateUser(id);
    }

}