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
        'memories',
        'spaces',
        'sessions',
        'resources',
        'audio',
        'images',
        'auth',
        'errors',
        'types',
        'deps',
      ],
    ],
    'subject-case': [2, 'always', 'sentence-case'],
    'body-max-line-length': [1, 'always', 100],
  },
};
