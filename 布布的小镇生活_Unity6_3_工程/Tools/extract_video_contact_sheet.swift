import AVFoundation
import AppKit
import Foundation

if CommandLine.arguments.count < 4 {
    print("usage: extract_video_contact_sheet.swift <video> <output-folder> <frame-count>")
    exit(2)
}

let videoURL = URL(fileURLWithPath: CommandLine.arguments[1])
let outputFolder = URL(fileURLWithPath: CommandLine.arguments[2], isDirectory: true)
let frameCount = max(1, Int(CommandLine.arguments[3]) ?? 24)
try FileManager.default.createDirectory(at: outputFolder, withIntermediateDirectories: true)

let asset = AVAsset(url: videoURL)
let durationSeconds = CMTimeGetSeconds(asset.duration)
guard durationSeconds.isFinite, durationSeconds > 0 else {
    print("invalid duration")
    exit(1)
}

let generator = AVAssetImageGenerator(asset: asset)
generator.appliesPreferredTrackTransform = true
generator.maximumSize = CGSize(width: 640, height: 360)
generator.requestedTimeToleranceBefore = CMTime(seconds: 0.4, preferredTimescale: 600)
generator.requestedTimeToleranceAfter = CMTime(seconds: 0.4, preferredTimescale: 600)

var frames: [NSImage] = []
var labels: [String] = []

for i in 0..<frameCount {
    let t = durationSeconds * (Double(i) + 0.5) / Double(frameCount)
    let time = CMTime(seconds: t, preferredTimescale: 600)
    do {
        let cgImage = try generator.copyCGImage(at: time, actualTime: nil)
        let image = NSImage(cgImage: cgImage, size: NSSize(width: cgImage.width, height: cgImage.height))
        frames.append(image)
        labels.append(String(format: "%02d  %.1fs", i + 1, t))

        let frameURL = outputFolder.appendingPathComponent(String(format: "frame_%02d.jpg", i + 1))
        if let tiff = image.tiffRepresentation,
           let bitmap = NSBitmapImageRep(data: tiff),
           let data = bitmap.representation(using: .jpeg, properties: [.compressionFactor: 0.82]) {
            try data.write(to: frameURL)
        }
    } catch {
        print("frame \(i + 1) failed: \(error)")
    }
}

let columns = 4
let thumbWidth = 400
let thumbHeight = 225
let labelHeight = 34
let rows = Int(ceil(Double(frames.count) / Double(columns)))
let sheetSize = NSSize(width: columns * thumbWidth, height: rows * (thumbHeight + labelHeight))
let sheet = NSImage(size: sheetSize)
sheet.lockFocus()
NSColor(calibratedWhite: 0.10, alpha: 1).setFill()
NSRect(origin: .zero, size: sheetSize).fill()

let paragraph = NSMutableParagraphStyle()
paragraph.alignment = .center
let attrs: [NSAttributedString.Key: Any] = [
    .font: NSFont.systemFont(ofSize: 18, weight: .medium),
    .foregroundColor: NSColor.white,
    .paragraphStyle: paragraph
]

for (index, frame) in frames.enumerated() {
    let col = index % columns
    let row = rows - 1 - index / columns
    let x = col * thumbWidth
    let y = row * (thumbHeight + labelHeight)
    frame.draw(in: NSRect(x: x, y: y + labelHeight, width: thumbWidth, height: thumbHeight))
    labels[index].draw(in: NSRect(x: x, y: y + 6, width: thumbWidth, height: labelHeight - 8), withAttributes: attrs)
}

sheet.unlockFocus()
let sheetURL = outputFolder.appendingPathComponent("contact_sheet.jpg")
if let tiff = sheet.tiffRepresentation,
   let bitmap = NSBitmapImageRep(data: tiff),
   let data = bitmap.representation(using: .jpeg, properties: [.compressionFactor: 0.88]) {
    try data.write(to: sheetURL)
    print(sheetURL.path)
}
