﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

public class FileSystemNode : TreeViewItem {
    public string NodeName { get; set; }

    public string FullPath { get; set; }

    public void Fill() {
        TryGetDirectories(FullPath)?.ToList().ForEach(d => {
            AddChild(Path.GetFileName(d), d);
            Fill(Find(d), d);
        });
    }

    private void Fill(FileSystemNode node, string path) {
        TryGetDirectories(path)?.ToList().ForEach(d => node.AddChild(Path.GetFileName(d), d));
    }

    public void AddChild(string newName, string newPath) {
        var add = new FileSystemNode {Header = newName, NodeName = newName, FullPath = newPath};
        add.Expanded += Node_Expand;
        Items.Add(add);
    }

    public void AppendTree(FileSystemNode node) {
        TryGetDirectories(node.FullPath)?.ToList().ForEach(d => {
            var child = Find(node, d);
            if (null != child) AppendTree(child, d);
        });
    }

    private void AppendTree(FileSystemNode node, string path) {
        TryGetDirectories(path)?.Where(d => !node.ChildExists(d)).ToList()
            .ForEach(d => node.AddChild(Path.GetFileName(d), d));
    }

    private IEnumerable<string> TryGetDirectories(string path) {
        try {
            return Directory.EnumerateDirectories(path);
        }
        catch (UnauthorizedAccessException) {
            return null;
        }
    }

    public bool ChildExists(string otherPath) {
        return Items.Cast<FileSystemNode>().Any(child => child.Equals(otherPath));
    }

    public bool Equals(string otherPath) {
        return FullPath.Equals(otherPath);
    }

    private FileSystemNode Find(string otherPath) {
        return Items.Cast<FileSystemNode>().FirstOrDefault(child => child.Equals(otherPath));
    }

    private FileSystemNode Find(FileSystemNode node, string otherPath) {
        return node.Items.Cast<FileSystemNode>().FirstOrDefault(child => child.Equals(otherPath));
    }

    private void Node_Expand(object sender, RoutedEventArgs e) {
        try {
            AppendTree(sender as FileSystemNode);
        }
        catch (Exception ex) {
            Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
        }
    }
}