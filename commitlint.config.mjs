export default {
  extends: ['@commitlint/config-conventional'],
  rules: {
    'type-enum': [
      2,
      'always',
      [
        'feat',     // New feature
        'fix',      // Bug fix
        'docs',     // Documentation only
        'style',    // Formatting, missing semi colons
        'refactor', // Code change that neither fixes a bug nor adds a feature
        'perf',     // Performance improvement
        'test',     // Adding tests
        'chore',    // Updating build tasks, package manager configs
        'revert',   // Reverting a previous commit
        'build',    // Changes to build system or dependencies
        'ci',       // CI configuration changes
      ],
    ],
    'scope-enum': [
      2,
      'always',
      [
        'client',
        'types',
        'errors',
        'utils',
        'models',

        // Resource-specific scopes (match actual structure)
        'memories',
        'spaces',
        'sessions',
        'search',
        'graph',
        'entities',
        'facts',
        'relationships',
        'highlights',
        'enrichments',
        'feedback',
        'agent',
        'jobs',
        'clusters',
        'resources',
        'webhooks',

        // Infrastructure
        'auth',
        'deps',
        'config',
        'build',
      ],
    ],
    'subject-case': [2, 'always', 'sentence-case'],
    'body-max-line-length': [1, 'always', 100],
  },
};
