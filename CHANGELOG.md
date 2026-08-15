# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.6.1](https://github.com/trixdb/trix-sdk-csharp/compare/v0.6.0...v0.6.1) (2026-08-15)


### Features

* add correlation ID and slow request warning to HttpPipeline ([6b3f7b5](https://github.com/trixdb/trix-sdk-csharp/commit/6b3f7b5ea57fda7ec0aa42145175c7c69a25f9e2))
* add GetActiveBranchesAsync to C# SDK — BranchInfo + ActiveBranchesResult models ([b80ce6e](https://github.com/trixdb/trix-sdk-csharp/commit/b80ce6e82e67683e6f0beb590e8ddb719a033253))
* add GetGoalProgressHistoryAsync + GoalProgressEvent/GoalProgressHistoryResponse models ([b15ac7a](https://github.com/trixdb/trix-sdk-csharp/commit/b15ac7ae636a0b724f484a96f85fb07a5275b00a))
* Add image memory support to C# SDK ([e3b63bf](https://github.com/trixdb/trix-sdk-csharp/commit/e3b63bf956bc5dba551e7a1d0abab9bbfe357f99))
* add missing DTO fields to Memory and CreateMemoryRequest ([339d3a5](https://github.com/trixdb/trix-sdk-csharp/commit/339d3a594b93a92af6a5e19a9022579f109bd18a))
* add pinning, protection, topics, quality, multi-scale support ([c9a8af5](https://github.com/trixdb/trix-sdk-csharp/commit/c9a8af582c98d0fd4473df65bf9f28c5bb7012ee))
* add PrFileMetric model to PrReviewResult ([93407d7](https://github.com/trixdb/trix-sdk-csharp/commit/93407d7604ceb3f7eef18bcf8baa03e30d91ce19))
* Add Resources API, fix bulk operations, add origin/context fields ([53966b3](https://github.com/trixdb/trix-sdk-csharp/commit/53966b31ec0e880c6c54af45956d1b18d4da72d0))
* add space config resource to C# SDK ([678e5e4](https://github.com/trixdb/trix-sdk-csharp/commit/678e5e490d903862b76b4162e4d382de30a57d7e))
* Add TasksResource (proof-of-concept for coverage gap [#8](https://github.com/trixdb/trix-sdk-csharp/issues/8)) ([#21](https://github.com/trixdb/trix-sdk-csharp/issues/21)) ([244e90e](https://github.com/trixdb/trix-sdk-csharp/commit/244e90efbc83fbde8df37f2f164e4137a9bf178d))
* add Webhooks.VerifySignature for inbound webhook verification ([#25](https://github.com/trixdb/trix-sdk-csharp/issues/25)) ([ca22322](https://github.com/trixdb/trix-sdk-csharp/commit/ca22322c7bd9258b8e6af97ce1b850fa4974bc0b))
* **ADR-109a:** add AgentResource.ResolvePipelineAsync() to C# SDK (tick 79) ([d186660](https://github.com/trixdb/trix-sdk-csharp/commit/d186660b2c7b59b73d25bfe4374f32cfc16e9333))
* **ADR-143:** add TrixClient.PingAsync() health check ([a9cb0bb](https://github.com/trixdb/trix-sdk-csharp/commit/a9cb0bbc6dc391e8b634eec01afb11fa1a4ed6f9))
* **build:** add SourceLink package and fix commitlint scopes ([2274fe2](https://github.com/trixdb/trix-sdk-csharp/commit/2274fe2723bfc4ce54f1207227cb537ef57e9f08))
* **ci:** add release please automation for automated changelogs ([31cd2cb](https://github.com/trixdb/trix-sdk-csharp/commit/31cd2cb50ac9335ec7c3e8d244eb2332da6c96bd))
* **client:** Block private/loopback/link-local BaseUrl hosts (SSRF guard) ([#19](https://github.com/trixdb/trix-sdk-csharp/issues/19)) ([9f7287a](https://github.com/trixdb/trix-sdk-csharp/commit/9f7287a76df4ecb1f196cefd8ee126d82f1390a6))
* **csharp-sdk:** add 6 code health methods and model classes ([c34006d](https://github.com/trixdb/trix-sdk-csharp/commit/c34006d3f69d2c57a486ca73edf83ae7b30fc7a6))
* **csharp-sdk:** add audio/video transcription features with diarization ([4ed5035](https://github.com/trixdb/trix-sdk-csharp/commit/4ed5035ba144e56ba41a81792afc4b802c1ae52d))
* **csharp-sdk:** add calendar resource (ADR-075 Phase 3) ([47a3019](https://github.com/trixdb/trix-sdk-csharp/commit/47a30198c9a9f1d61aba2f7a7478312b074adebb))
* **csharp-sdk:** add graph expansion with hybrid scoring ([e95e010](https://github.com/trixdb/trix-sdk-csharp/commit/e95e010eb038fed6e1e45b941bdf9efedbefd8ae))
* **csharp-sdk:** add HubRoles resource for custom role management (ADR-080) ([1e108de](https://github.com/trixdb/trix-sdk-csharp/commit/1e108de77ec86836e148466c82a98f6dd11e2686))
* **github:** add AgentAuditResult models + GetAgentAuditTrailAsync ([ec6c485](https://github.com/trixdb/trix-sdk-csharp/commit/ec6c48532aae753384f99a0a82f1bad968fbd91b))
* **github:** add AgentQualityScores + HumanAvgQuality to AgentAttributionResponse ([2c585d8](https://github.com/trixdb/trix-sdk-csharp/commit/2c585d80a0f032a10ce2c24c9a2f0b9ce03ba733))
* **github:** add ApprovedPR models and GetApprovedPRsAsync method ([84522a7](https://github.com/trixdb/trix-sdk-csharp/commit/84522a7de75fd5af37d935c8bb925b542e2a22a7))
* **github:** add AssigneeCycleTimeResult + GetAssigneeCycleTimeAsync ([a3c7fb3](https://github.com/trixdb/trix-sdk-csharp/commit/a3c7fb374ed4f4b9f9b98eea22e25e4482f8e39d))
* **github:** add CycleTimeTrendResult + GetCycleTimeTrendAsync (Phase 4) ([d4b4c63](https://github.com/trixdb/trix-sdk-csharp/commit/d4b4c63ac3e49e0aabe6de3455339a2de4951dc3))
* **github:** add GetIssueFlowAsync() and IssueFlowResult model ([0cf4828](https://github.com/trixdb/trix-sdk-csharp/commit/0cf48280595e4cce41ca854e6bc17a01fda976bd))
* **github:** add GetIssueTriageAsync() and TriageIssue models ([f6bbc03](https://github.com/trixdb/trix-sdk-csharp/commit/f6bbc032696d8aa6bdd730889d3ecc1bffbc8bb3))
* **github:** add GetPrQualityTrendAsync — 12-week PR quality score trend ([ed864df](https://github.com/trixdb/trix-sdk-csharp/commit/ed864dff1bbe0a79a5fdb3085cc24967db87d5a9))
* **github:** add GetPrSizeDistributionAsync to C# SDK (ADR-152) ([9fbd776](https://github.com/trixdb/trix-sdk-csharp/commit/9fbd77664ed9f57d89d1aa273eb19308dc51c30e))
* **github:** add GetReviewTurnaroundAsync to C# SDK (ADR-152) ([bdc7fff](https://github.com/trixdb/trix-sdk-csharp/commit/bdc7ffff102ccfd87bbeb03f2af66579b768af15))
* **github:** add HealthSnapshotIssueFlow model + IssueFlow field ([599db60](https://github.com/trixdb/trix-sdk-csharp/commit/599db60e44cf3011406972afe9761559f7678aca))
* **github:** add issue cycle time types and GetIssueCycleTimeAsync (Phase 4) ([37fc8ac](https://github.com/trixdb/trix-sdk-csharp/commit/37fc8ac2c2289eeb37e29b33e599917454fa9c8d))
* **github:** add IssueBacklog + ReviewCoverage to HealthSnapshotResponse ([e128ed7](https://github.com/trixdb/trix-sdk-csharp/commit/e128ed7663543c5ef365e5648bc1b640c8ddca80))
* **github:** add IssueBacklogResult model and GetIssueBacklogAsync method ([10b9ad0](https://github.com/trixdb/trix-sdk-csharp/commit/10b9ad0009643bd15c8134d810d4e8eb83d50511))
* **github:** add IssueResolversResult + GetIssueResolversAsync (Phase 4) ([abe6dbf](https://github.com/trixdb/trix-sdk-csharp/commit/abe6dbf2ab97e98cea07934a0600bbe618471c09))
* **github:** add IssueThroughput and SlowestCycleLabel to HealthSnapshotResponse ([4085e3c](https://github.com/trixdb/trix-sdk-csharp/commit/4085e3c550f2285d5b65dbef17661dd5005c52f4))
* **github:** add IssueThroughputResult + GetIssueThroughputAsync (Phase 4) ([e8bd0b5](https://github.com/trixdb/trix-sdk-csharp/commit/e8bd0b538e9646cc96b7868ea04a1c1540d69ada))
* **github:** add minQualityScore/maxQualityScore to GetPrBriefsAsync ([520214b](https://github.com/trixdb/trix-sdk-csharp/commit/520214b32ecf0f5eb6e94eae97e868abce8d7072))
* **github:** add review network C# SDK support ([a43a409](https://github.com/trixdb/trix-sdk-csharp/commit/a43a409bfeaee1df3510899d7a9126b47d0b2914))
* **github:** add ReviewCoverageResult model and GetReviewCoverageAsync method ([ed7c5ed](https://github.com/trixdb/trix-sdk-csharp/commit/ed7c5ed6ec0ed9a04a79d9e978246446ca3640af))
* **github:** add reviewTurnaround/urgentItems/prQualityTrend to HealthSnapshot; reviewsGiven/approvals to ContributorQualityStat; requestedReviewers/hasReview to OpenPRAging ([4650966](https://github.com/trixdb/trix-sdk-csharp/commit/4650966a3dc23d6c9637d252b248a0c92dd7c4f2))
* **github:** add ScopeCreepResult models + GetScopeCreepAsync ([36f2766](https://github.com/trixdb/trix-sdk-csharp/commit/36f2766c2be3398261dd302a0e58b2b72a633309))
* **github:** add WorkQueueItem/Result models and GetWorkQueueAsync() (ADR-152) ([1d34919](https://github.com/trixdb/trix-sdk-csharp/commit/1d34919f095254c45d76fbd722fe2fd311728074))
* **github:** AssigneeStat + IssueAssigneesResult models; GetIssueAssigneesAsync() ([0251207](https://github.com/trixdb/trix-sdk-csharp/commit/0251207b8a0581be21d3432e0a40304457af1722))
* **github:** CommitLeader + CommitLeadersResult models; GetCommitLeadersAsync() ([b803432](https://github.com/trixdb/trix-sdk-csharp/commit/b8034329bb6029f7161f746413863fed3e59e3fc))
* **github:** ContributorMomentumResult model + GetContributorMomentumAsync method ([695b3b7](https://github.com/trixdb/trix-sdk-csharp/commit/695b3b70ea513869d0afc5fc1df07775b471e13f))
* **github:** GetAIvsHumanQualityAsync — AI vs human code quality comparison ([f35187d](https://github.com/trixdb/trix-sdk-csharp/commit/f35187dc23e97f20bd54a8392b55b853f0a35bc5))
* **github:** GetBusFactorAsync — knowledge concentration risk (ADR-152) ([2295efc](https://github.com/trixdb/trix-sdk-csharp/commit/2295efc911be7233ac9a2ef15d41f027e08b98cb))
* **github:** GetDORAMetricsAsync — DORA engineering excellence metrics ([4a6b752](https://github.com/trixdb/trix-sdk-csharp/commit/4a6b7522985f6a3197af4c9d0a8f5a474ac4e049))
* **github:** GetPRTaskAlignmentAsync — detect semantic drift between PRs and linked issues ([9f31d9c](https://github.com/trixdb/trix-sdk-csharp/commit/9f31d9ce0f217924e7e9c6b42683c1062bd1dd78))
* **github:** GetReviewDepthAsync — reviewer thoroughness analytics ([ee4785d](https://github.com/trixdb/trix-sdk-csharp/commit/ee4785d9c8c45594f2b2c8cabc4b8653b3fccd50))
* **github:** GetTestGapAsync — test coverage gap endpoint (ADR-152) ([1b726e4](https://github.com/trixdb/trix-sdk-csharp/commit/1b726e4dd3a4d79d72e46ddff88d4f745ebf92c7))
* **github:** LabelVelocity + LabelVelocityResult models; GetLabelVelocityAsync() ([395e3f2](https://github.com/trixdb/trix-sdk-csharp/commit/395e3f220bf045dc7592ceeb269d6ddcf51dfd24))
* **github:** MilestoneStat + MilestonesResult models; GetMilestonesAsync() ([a8f2ff8](https://github.com/trixdb/trix-sdk-csharp/commit/a8f2ff85d4f4d31d3c01eb9ca252a840bdac7276))
* **github:** PrMergeTimeResult model + GetPrMergeTimeAsync method ([53cd376](https://github.com/trixdb/trix-sdk-csharp/commit/53cd37685fe358022d6cfa712f9bbb1182b650bd))
* **github:** ReviewerWorkloadResult + GetReviewerWorkloadAsync ([dc36b2a](https://github.com/trixdb/trix-sdk-csharp/commit/dc36b2aed66efd387b74919d70d641fd1e4478c1))
* **github:** update ReleaseReadinessResponse models — richer structure (blockers, hotspots, stale PRs) ([9af69dc](https://github.com/trixdb/trix-sdk-csharp/commit/9af69dc3e2549df0cd1f0081c69964ec9412a1bf))
* **P10:** C# SDK account-default pipeline methods ([63c9de3](https://github.com/trixdb/trix-sdk-csharp/commit/63c9de3b1831adbbb36137e977e38e18bac3800b))
* **P10:** C# SDK space-default pipeline methods ([94ce4a5](https://github.com/trixdb/trix-sdk-csharp/commit/94ce4a5838d7b1270c0d5b0851263b7a4d15c0a7))
* **P10:** C# SDK trigger methods for session/mega/scoped ([86db2ff](https://github.com/trixdb/trix-sdk-csharp/commit/86db2ffc80ef2061d78351e901ed8f73ed76ad2a))
* **sdk-cs:** add GetHealthSnapshotAsync + HealthSnapshotResponse models ([9414b6e](https://github.com/trixdb/trix-sdk-csharp/commit/9414b6edbe906fff730b97964368593534f155f5))
* **sdk-cs:** add GetPrBriefsAsync + PRBrief/PRBriefsResponse models ([77aa40e](https://github.com/trixdb/trix-sdk-csharp/commit/77aa40ee4aa7085e83c0d63067b93d082cfb444d))
* **sdk-cs:** add PrUrl field to PRBrief model ([a4cb3e2](https://github.com/trixdb/trix-sdk-csharp/commit/a4cb3e2ab007ef59442707be8c39a9775cabd8b9))
* **sdk-csharp:** add batch_search, knowledge, store_and_organize, suggest_strategy ([bbae4fb](https://github.com/trixdb/trix-sdk-csharp/commit/bbae4fb7862c141265aa833ba5d0c439263da8ae))
* **sdk-csharp:** add code analysis models and PR review methods ([44fbcf6](https://github.com/trixdb/trix-sdk-csharp/commit/44fbcf6a6c1cec3baeef1769000e873b409ed8eb))
* **sdk-csharp:** add Phase 5 methods and models for code quality scanner ([f365376](https://github.com/trixdb/trix-sdk-csharp/commit/f3653765f14cbeb9fa515ee659accd19041461cc))
* **sdk-csharp:** filePath filter + CreateIssueFromSuggestionAsync ([1aea98e](https://github.com/trixdb/trix-sdk-csharp/commit/1aea98e95be134d8b7ead3bbf2a512ee2958d9a2))
* **sdk/csharp:** agent filter param for GetPrBriefsAsync ([f1e6b52](https://github.com/trixdb/trix-sdk-csharp/commit/f1e6b52deb83e383d4e8bd14d28abdb09aef269b))
* **sdk:** add AvgMergeDays to ContributorQualityStat ([96b9f33](https://github.com/trixdb/trix-sdk-csharp/commit/96b9f33f92866d58513302933a8c713ce27e48f9))
* **sdk:** add GetContributorQualityAsync to C# SDK ([3dc73a1](https://github.com/trixdb/trix-sdk-csharp/commit/3dc73a197c974fb8a7064e1e44d60548190426d4))
* **sdk:** add GetPrAgingAsync to C# SDK ([5ca1f8a](https://github.com/trixdb/trix-sdk-csharp/commit/5ca1f8aa12bac7bbfc4e79f25229199470cbeaad))
* **sdk:** add GetWeekOverWeekAsync — 7-day velocity comparison ([07f09ed](https://github.com/trixdb/trix-sdk-csharp/commit/07f09ed281550d7e4fe26e5b2cc8f7c4b2c074d3))
* **spaces:** add slug support with GetBySlugAsync method ([da60cf6](https://github.com/trixdb/trix-sdk-csharp/commit/da60cf628fef4e6690c6653d8115e86fe27d4337))
* **types:** add Agent field to PRBrief ([e84d361](https://github.com/trixdb/trix-sdk-csharp/commit/e84d361e0f1eebebbf44ec5766eb2ef7113b89a4))


### Bug Fixes

* add batch size validation (max 1000) to all bulk operations ([4e43174](https://github.com/trixdb/trix-sdk-csharp/commit/4e4317418b8ae1f3e8552fd5e8ce0619c1cee2d3))
* add CRLF injection validation for HTTP header values ([35a824e](https://github.com/trixdb/trix-sdk-csharp/commit/35a824e957940bde41a7c5ba1fd94abd7315d6e9))
* add DisposeAsync override to LeaveOpenStream ([f08c29f](https://github.com/trixdb/trix-sdk-csharp/commit/f08c29f87d2d5c3b686636a5a9a133fdcff98cbc))
* add pagination safety limit (max 1000 pages) to all ListAllAsync methods ([c8f7d9c](https://github.com/trixdb/trix-sdk-csharp/commit/c8f7d9cb0ea4fee06265f0372397f9423f4f01c6))
* add required Slug field to Space test fixture ([f40d5d7](https://github.com/trixdb/trix-sdk-csharp/commit/f40d5d75df5205a30fe9171ab02aae1d696c5a99))
* **ADR-145:** update C# example + graph test for snake_case wire contract ([1dd48fb](https://github.com/trixdb/trix-sdk-csharp/commit/1dd48fbd91478c2f20f5760e577f4ecb48da70b0))
* Align SDK with API for examples compatibility ([95b35f0](https://github.com/trixdb/trix-sdk-csharp/commit/95b35f050564f53c24da15f96603cd7fc2071ae4))
* **build:** Resolve CS1573/CS0109 warnings and guard against regressions ([#18](https://github.com/trixdb/trix-sdk-csharp/issues/18)) ([bbdcd81](https://github.com/trixdb/trix-sdk-csharp/commit/bbdcd81de995fc577b408e2d774ab2a578bf8daa))
* **client:** Dispose the HttpResponseMessage on error/retry paths ([#20](https://github.com/trixdb/trix-sdk-csharp/issues/20)) ([cb765c9](https://github.com/trixdb/trix-sdk-csharp/commit/cb765c999c338f027c782d183c4adbeb4d52fa76))
* **client:** Don't dispose a caller-supplied HttpMessageHandler ([#3](https://github.com/trixdb/trix-sdk-csharp/issues/3)) ([#13](https://github.com/trixdb/trix-sdk-csharp/issues/13)) ([d0e1ff4](https://github.com/trixdb/trix-sdk-csharp/commit/d0e1ff437da85132c0380781a54cc01a6307faa1))
* **client:** Send a stable Idempotency-Key across retries ([#12](https://github.com/trixdb/trix-sdk-csharp/issues/12)) ([ab65080](https://github.com/trixdb/trix-sdk-csharp/commit/ab6508081b26a3d79421af67dc467def8df69faf))
* **client:** Single-source SdkVersion from the assembly version ([#4](https://github.com/trixdb/trix-sdk-csharp/issues/4)) ([#15](https://github.com/trixdb/trix-sdk-csharp/issues/15)) ([920809d](https://github.com/trixdb/trix-sdk-csharp/commit/920809d2bacc92bd7c89dd80605c5c532624bfaa))
* correct CodeSummaryResult model to match actual API response ([3c13b13](https://github.com/trixdb/trix-sdk-csharp/commit/3c13b130fe4ea88eb9b4e0555511daf80873e280))
* correct enum JSON wire serialization to match the API ([#24](https://github.com/trixdb/trix-sdk-csharp/issues/24)) ([f4d12ed](https://github.com/trixdb/trix-sdk-csharp/commit/f4d12ed1b3d26e41ac910f47dc51a67122ebdea8))
* **csharp-sdk:** add JsonPropertyName attributes to calendar models ([a956697](https://github.com/trixdb/trix-sdk-csharp/commit/a9566972efd162b809e64fb9ed2aa2dd9b771973))
* **csharp-sdk:** add multi-node support to Graph.Traverse ([47e9447](https://github.com/trixdb/trix-sdk-csharp/commit/47e9447216beb6b1cfcc8576fbcce85a21fa5e8c))
* defensive copy of CustomHeaders to prevent post-construction mutation ([71bf625](https://github.com/trixdb/trix-sdk-csharp/commit/71bf625aa3a0d8c5fb1f3b29262cd63fa1befda8))
* improve NuGet publish command with proper secret handling ([f29064b](https://github.com/trixdb/trix-sdk-csharp/commit/f29064b286c3b1fbae76cc6a9ab45a6f375c637b))
* make ResponseOwningStream Dispose thread-safe with Interlocked.Exchange ([bfc7193](https://github.com/trixdb/trix-sdk-csharp/commit/bfc7193d3982aadd572afd1a6e301cb18094ad92))
* make TrixClient.Dispose() thread-safe with atomic Interlocked.Exchange ([c02296d](https://github.com/trixdb/trix-sdk-csharp/commit/c02296de49c2f5fcf72876aaccb1edc2ea444c94))
* Pass CancellationToken by name to fix 10 CS1503 build errors ([#1](https://github.com/trixdb/trix-sdk-csharp/issues/1)) ([#9](https://github.com/trixdb/trix-sdk-csharp/issues/9)) ([bc73e81](https://github.com/trixdb/trix-sdk-csharp/commit/bc73e81facb25f468cc4c22e3e5726cacb546ed8))
* prevent response leak in GetStreamAsync if ReadAsStreamAsync throws ([dc979e6](https://github.com/trixdb/trix-sdk-csharp/commit/dc979e6f49f83006c9e075d1c18cc9d0ee598e91))
* production-readiness improvements with comprehensive test coverage ([3455144](https://github.com/trixdb/trix-sdk-csharp/commit/34551447ffbb1995c9d6ed7b6bdaadd6f8df2b9e))
* replace bare catch with typed catch and debug logging in ReadErrorBodyAsync ([1fe3936](https://github.com/trixdb/trix-sdk-csharp/commit/1fe393621985e09b56225dee1bf5f7df4dca53f1))
* resolve formatting and xUnit1031 warnings in Spaces tests ([439c4c5](https://github.com/trixdb/trix-sdk-csharp/commit/439c4c5810fcc6c208cf50a246221c915e55ca25))
* resolve ObjectDisposedException in Spaces tests ([fcbc8fa](https://github.com/trixdb/trix-sdk-csharp/commit/fcbc8fa677c70d6cfdce844baff83c135fc1f618))
* update repository URLs to trixdb org ([782d8f0](https://github.com/trixdb/trix-sdk-csharp/commit/782d8f065c924c7d2763bde24d4c3a943af54ab9))
* URL-encode ID parameters in resource path interpolations ([99699f0](https://github.com/trixdb/trix-sdk-csharp/commit/99699f00b991d322ae4c98adb2b24e51a72a1524))
* use static HttpMethod.Patch instead of allocating new HttpMethod("PATCH") ([41fe047](https://github.com/trixdb/trix-sdk-csharp/commit/41fe0471c39fe15cf22c68307417d6a99879a7fe))
* validate response Content-Length before deserializing (50MB max) ([7e6d6a8](https://github.com/trixdb/trix-sdk-csharp/commit/7e6d6a843b9d8eee8cc396b565855b2b25dd3cb3))


### Documentation

* align README to canon + document webhook verification ([#26](https://github.com/trixdb/trix-sdk-csharp/issues/26)) ([1a9c60f](https://github.com/trixdb/trix-sdk-csharp/commit/1a9c60fa8b3188c76354aaad105231aca02f08d6))
* fix package name in README (Trix.Client → Trix) ([85b8e18](https://github.com/trixdb/trix-sdk-csharp/commit/85b8e18a47c5bc04266721b8de0286039cd048e0))
* Fix stale requirements, wrong SDK links, and a non-compiling example ([#22](https://github.com/trixdb/trix-sdk-csharp/issues/22)) ([c9b0d17](https://github.com/trixdb/trix-sdk-csharp/commit/c9b0d17030de5d018d1105bda9828b4d17fcd9a3))

## [0.6.0] - 2025-12-27

### Added

- Initial release of Trix SDK for .NET
- **TrixClient**: Main client for API interaction
  - Support for API key and JWT authentication
  - Configurable timeouts and retry policies
  - Environment variable configuration
- **MemoriesResource**: Full CRUD operations for memories
  - Create, get, update, delete memories
  - List with filters (search, tags, type, space)
  - Bulk create operations
  - Async enumerable for pagination
- **RelationshipsResource**: Relationship management
  - Create relationships between memories
  - Reinforce and weaken relationships
  - Get incoming/outgoing relationships
- **ClustersResource**: Cluster operations
  - Create and manage clusters
  - Add/remove memories from clusters
  - Cluster expansion suggestions
- **SpacesResource**: Workspace management
  - Create and manage spaces
- **Models**: Strongly typed models
  - Memory, Relationship, Cluster, Space
  - Fact, Entity for knowledge graph
  - Paginated responses
- **Exceptions**: Typed exception hierarchy
  - TrixException base class
  - AuthenticationException, PermissionException
  - NotFoundException, ValidationException
  - RateLimitException, ServerException
  - NetworkException, TrixTimeoutException
- **Resilience**: Built-in retry with exponential backoff
- **Cancellation**: Full CancellationToken support
- **Logging**: Microsoft.Extensions.Logging integration

### Requirements

- .NET 8.0 or later (the package multi-targets `net8.0` and `net10.0`)
