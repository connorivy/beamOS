import commonjs from '@rollup/plugin-commonjs';
import { nodeResolve } from '@rollup/plugin-node-resolve';
import terser from '@rollup/plugin-terser';
import multi from '@rollup/plugin-multi-entry';

export default {
  input: 'Features/**/*.js', // Update the path to match all your JS files
  output: {
    file: 'wwwroot/js/beamos.min.js',
    format: 'iife', // Immediately Invoked Function Expression
    name: 'BeamOsComponents'
  },
  plugins: [
    multi(),
    nodeResolve(),
    commonjs(),
    terser()
  ]
};
