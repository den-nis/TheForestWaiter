namespace TheForestWaiter.Performance;

enum ProfileCategory
{
	None,

	Tick,

	UpdateGame,

	UpdateWorldParticles,
	UpdateCreatures,
	UpdateProjectiles,
	UpdateEnvironment,
	UpdateOther,

	DrawGame,

	DrawWorldBackground,
	DrawWorldForeground,
	DrawWorldParticles,

	DrawCreatures,
	DrawProjectiles,
	DrawEnvironment,
	DrawOther,
	DrawHud,
	DrawBackground,
}
