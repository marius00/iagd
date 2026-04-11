import { defineConfig } from 'vite';
import preact from '@preact/preset-vite';
import path from 'path';
import fs from 'fs';

// Serve src/static as /static/ in dev mode (game icons, not committed to git).
// In production the C# host provides the static folder from its resources.
const serveStaticInDev = () => ({
  name: 'serve-src-static',
  configureServer(server: any) {
    const staticDir = path.resolve(process.cwd(), 'src/static');
    server.middlewares.use('/static/', (req: any, res: any, next: any) => {
      if (!fs.existsSync(staticDir)) return next();
      const filePath = path.join(staticDir, decodeURIComponent(req.url || ''));
      try {
        if (fs.existsSync(filePath) && fs.statSync(filePath).isFile()) {
          fs.createReadStream(filePath).pipe(res);
        } else {
          next();
        }
      } catch {
        next();
      }
    });
  },
});

export default defineConfig({
  plugins: [preact(), serveStaticInDev()],
  base: './',
  build: {
    outDir: 'build',
  },
  server: {
    port: 3000,
  },
});

